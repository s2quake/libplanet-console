using System.Collections.Immutable;
using System.Numerics;
using System.Reflection;
using Libplanet.Action.State;
using Libplanet.Types.Consensus;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Actions;

namespace LibplanetConsole.Node.Hand;

internal sealed class ActionProvider : IActionProvider
{
    public const string CurrencyCode = "won";

    public static Currency Currency { get; } = Currency.Uncapped("KRW", 2, null);

    public ImmutableArray<IAction> BeginBlockActions { get; } = [];

    public ImmutableArray<IAction> EndBlockActions { get; } = [];

    public ImmutableArray<IAction> BeginTxActions { get; } = [];

    public ImmutableArray<IAction> EndTxActions { get; } = [];

    public IAction[] GetGenesisActions(Address genesisAddress, PublicKey[] validatorKeys)
    {
        var validators = validatorKeys
            .Select(item => new Validator(item, BigInteger.One))
            .ToArray();
        var validatorSet = new ValidatorSet(validators: [.. validators]);
        var actions = new IAction[]
        {
            new Libplanet.Action.Sys.Initialize(
                validatorSet: validatorSet,
                states: ImmutableDictionary.Create<Address, IValue>()),
            new MintAction
            {
                Recipient = genesisAddress,
                Amount = Currency * 100_000_000_000,
            },
        };

        return actions;
    }

    public IActionLoader GetActionLoader()
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        var assemblies = GetAssemblies(executingAssembly);
        var actionLoaders = assemblies.Select(item => new AssemblyActionLoader(item)).ToArray();
        return new AggregateTypedActionLoader(actionLoaders);
    }

    private static IEnumerable<Assembly> GetAssemblies(Assembly assembly)
    {
        var directory = Path.GetDirectoryName(assembly.Location)!;
        var files = Directory.GetFiles(directory, "LibplanetConsole.*.dll");
        string[] paths =
        [
            assembly.Location,
            .. files,
        ];
        return [.. paths.Distinct().Order().Select(Assembly.LoadFrom)];
    }

    [ActionType("mint_action")]
    private sealed class MintAction : ActionBase
    {
        public Address Recipient { get; set; }

        public FungibleAssetValue Amount { get; set; }

        protected override void OnLoadPlainValue(Dictionary values)
        {
            Recipient = new Address(values["recipient"]);
            Amount = new FungibleAssetValue(values["amount"]);
        }

        protected override Dictionary OnInitialize(Dictionary values)
        {
            return values.Add("recipient", Recipient.Bencoded)
                .Add("amount", Amount.Serialize());
        }

        protected override IWorld OnExecute(IActionContext context)
        {
            var world = context.PreviousState;
            var recipient = Recipient;
            var amount = Amount;
            return world.MintAsset(context, recipient, amount);
        }
    }
}
