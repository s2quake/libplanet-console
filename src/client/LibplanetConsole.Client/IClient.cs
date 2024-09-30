using Libplanet.Action;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.Node;

namespace LibplanetConsole.Client;

public interface IClient : IVerifier
{
    event EventHandler? Started;

    event EventHandler<StopEventArgs>? Stopped;

    ClientInfo Info { get; }

    NodeInfo NodeInfo { get; }

    bool IsRunning { get; }

    PublicKey PublicKey { get; }

    Address Address { get; }

    EndPoint NodeEndPoint { get; set; }

    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);

    Task<TxId> SendTransactionAsync(IAction[] actions, CancellationToken cancellationToken);
}
