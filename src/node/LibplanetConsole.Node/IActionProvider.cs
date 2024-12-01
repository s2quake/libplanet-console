using System.Collections.Immutable;

namespace LibplanetConsole.Node;

public interface IActionProvider
{
    ImmutableArray<IAction> BeginBlockActions { get; }

    ImmutableArray<IAction> EndBlockActions { get; }

    ImmutableArray<IAction> BeginTxActions { get; }

    ImmutableArray<IAction> EndTxActions { get; }

    IAction[] GetGenesisActions(Address genesisAddress, PublicKey[] validatorKeys);

    IActionLoader GetActionLoader();
}
