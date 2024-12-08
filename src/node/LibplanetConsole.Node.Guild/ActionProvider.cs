using System.Collections.Immutable;
using System.Numerics;
using System.Reflection;
using Libplanet.Types.Consensus;
using LibplanetConsole.Common.Actions;
using LibplanetConsole.Node.Guild.Actions;
using Nekoyume.Action.Loader;
using Nekoyume.Action.ValidatorDelegation;

namespace LibplanetConsole.Node.Guild;

internal sealed class ActionProvider : IActionProvider
{
    private static readonly BigInteger DefaultPower = BigInteger.Pow(10, 20);

    public ImmutableArray<IAction> BeginBlockActions { get; } =
    [
        new SlashValidator(),
        new AllocateGuildReward(),
        new AllocateReward(),
    ];

    public ImmutableArray<IAction> EndBlockActions { get; } =
    [
        new UpdateValidators(),
        new RecordProposer(),
        new RewardGoldAction(),
        new ReleaseValidatorUnbondings(),
    ];

    public ImmutableArray<IAction> BeginTxActions { get; } =
    [
        new Mortgage(),
    ];

    public ImmutableArray<IAction> EndTxActions { get; } =
    [
        new Reward(),
        new Refund(),
    ];

    public IActionLoader GetActionLoader()
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        var assemblies = GetAssemblies(executingAssembly);
        IActionLoader[] actionLoaders =
        [
            new NCActionLoader(),
            .. assemblies.Select(item => new AssemblyActionLoader(item)),
        ];
        return new AggregateTypedActionLoader(actionLoaders);
    }

    public IAction[] GetGenesisActions(Address genesisAddress, PublicKey[] validatorKeys)
    {
        var validators = validatorKeys
            .Select((item, i) => new Validator(item, i == 0 ? DefaultPower * 100 : DefaultPower))
            .ToArray();
        var action = new InitializeWorld
        {
            Validators = validators,
            GoldCurrency = Currency.Uncapped("NCG", 2, genesisAddress),
            GenesisAddress = genesisAddress,
        };

        return [action];
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
}
