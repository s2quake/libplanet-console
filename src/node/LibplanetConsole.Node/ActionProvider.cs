using System.Collections.Immutable;
using System.Numerics;
using System.Reflection;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.Loader;
using Libplanet.Crypto;
using Libplanet.Types.Consensus;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Actions;
using LibplanetConsole.Framework;

namespace LibplanetConsole.Node;

internal sealed class ActionProvider : IActionProvider
{
    public static ActionProvider Default { get; } = new();

    public ImmutableArray<IAction> BeginBlockActions { get; } = [];

    public ImmutableArray<IAction> EndBlockActions { get; } = [];

    public ImmutableArray<IAction> BeginTxActions { get; } = [];

    public ImmutableArray<IAction> EndTxActions { get; } = [];

    public IAction[] GetGenesisActions(AppPrivateKey genesisKey, PublicKey[] validatorKeys)
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
        };

        return actions;
    }

    public IActionLoader GetActionLoader()
    {
        var assemblies = ApplicationContainer.GetAssemblies(Assembly.GetExecutingAssembly());
        var actionLoaders = assemblies.Select(item => new AssemblyActionLoader(item)).ToArray();
        return new AggregateTypedActionLoader(actionLoaders);
    }
}
