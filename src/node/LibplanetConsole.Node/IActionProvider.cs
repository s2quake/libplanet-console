using System.Collections.Immutable;
using Libplanet.Action;
using Libplanet.Action.Loader;
using LibplanetConsole.Common;

namespace LibplanetConsole.Node;

public interface IActionProvider
{
    ImmutableArray<IAction> BeginBlockActions { get; }

    ImmutableArray<IAction> EndBlockActions { get; }

    ImmutableArray<IAction> BeginTxActions { get; }

    ImmutableArray<IAction> EndTxActions { get; }

    IAction[] GetGenesisActions(AppPrivateKey genesisKey, PublicKey[] validatorKeys);

    IActionLoader GetActionLoader();
}
