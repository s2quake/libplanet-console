using System.Collections.Immutable;
using System.Reflection;
using Libplanet.Types.Consensus;
using LibplanetConsole.Common.Actions;
using LibplanetConsole.Framework;
using Nekoyume.Action;
using Nekoyume.Action.Loader;
using Nekoyume.Action.ValidatorDelegation;

namespace LibplanetConsole.Node.Delegation;

internal sealed class ActionProvider : IActionProvider
{
    private const int DefaultPower = 1000;

    public ImmutableArray<IAction> BeginBlockActions { get; } =
    [
        new SlashValidator(),
        new AllocateReward(),
    ];

    public ImmutableArray<IAction> EndBlockActions { get; } =
    [
        new UpdateValidators(),
        new RecordProposer(),
        new RewardGold(),
        new ReleaseValidatorUnbondings(),
    ];

    public ImmutableArray<IAction> BeginTxActions { get; } = [];

    public ImmutableArray<IAction> EndTxActions { get; } = [];

    public IActionLoader GetActionLoader()
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        var assemblies = ApplicationServiceCollection.GetAssemblies(executingAssembly);
        IActionLoader[] actionLoaders =
        [
            new NCActionLoader(),
            .. assemblies.Select(item => new AssemblyActionLoader(item)),
        ];
        return new AggregateTypedActionLoader(actionLoaders);
    }

    public IAction[] GetGenesisActions(PrivateKey genesisKey, PublicKey[] validatorKeys)
    {
        var validators = validatorKeys
            .Select(item => new Validator(item, DefaultPower))
            .ToArray();
        var action = new InitializeStates
        {
            Validators = validators,
            GoldCurrency = Currency.Uncapped("NCG", 2, genesisKey.Address),
        };

        return [action];
    }
}
