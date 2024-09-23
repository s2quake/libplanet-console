using Libplanet.Action;
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

    AppPublicKey PublicKey { get; }

    AppAddress Address { get; }

    AppEndPoint NodeEndPoint { get; set; }

    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);

    Task<AppId> SendTransactionAsync(IAction[] actions, CancellationToken cancellationToken);
}
