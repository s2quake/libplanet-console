using System.Collections.Immutable;
using System.Numerics;
using System.Reflection;
using Libplanet.Action;
using Libplanet.Action.Loader;
using Libplanet.Crypto;
using Libplanet.Types.Assets;
using Libplanet.Types.Consensus;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Actions;
using LibplanetConsole.Framework;
using Nekoyume.Action;
using Nekoyume.Action.Loader;
using Nekoyume.Action.ValidatorDelegation;

namespace LibplanetConsole.Node.Delegation;

internal sealed class ActionProvider : IActionProvider
{
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
        var assemblies = ApplicationContainer.GetAssemblies(Assembly.GetExecutingAssembly());
        IActionLoader[] actionLoaders =
        [
            new NCActionLoader(),
            .. assemblies.Select(item => new AssemblyActionLoader(item)),
        ];
        return new AggregateTypedActionLoader(actionLoaders);
    }

    public IAction[] GetGenesisActions(AppPrivateKey genesisKey, PublicKey[] validatorKeys)
    {
        var validators = validatorKeys
            .Select(item => new Validator((PublicKey)item, BigInteger.One))
            .ToArray();
        var action = new InitializeStates
        {
            Validators = validators,
            GoldCurrency = Currency.Uncapped("NCG", 2, genesisKey.Address),
        };

        return [action];
    }
}
