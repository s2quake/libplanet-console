using Libplanet.Action;
using LibplanetConsole.Clients.Serializations;
using LibplanetConsole.Common;
using LibplanetConsole.Nodes.Serializations;

namespace LibplanetConsole.Clients;

public interface IClient : IVerifier
{
    event EventHandler<BlockEventArgs>? BlockAppended;

    event EventHandler? Started;

    event EventHandler<StopEventArgs>? Stopped;

    ClientInfo Info { get; }

    NodeInfo NodeInfo { get; }

    bool IsRunning { get; }

    AppPublicKey PublicKey { get; }

    AppAddress Address { get; }

    AppEndPoint NodeEndPoint { get; set; }

    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);

    Task<AppId> SendTransactionAsync(IAction[] actions, CancellationToken cancellationToken);
}
