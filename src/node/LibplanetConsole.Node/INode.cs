using LibplanetConsole.Common;

namespace LibplanetConsole.Node;

public interface INode : IVerifier, ISigner, IServiceProvider
{
    event EventHandler? Started;

    event EventHandler? Stopped;

    NodeInfo Info { get; }

    bool IsRunning { get; }

    PublicKey PublicKey { get; }

    Address Address => PublicKey.Address;

    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);

    Task<TxId> SendTransactionAsync(IAction[] actions, CancellationToken cancellationToken);
}
