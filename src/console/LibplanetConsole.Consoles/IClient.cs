using LibplanetConsole.Clients;
using LibplanetConsole.Common;

namespace LibplanetConsole.Consoles;

public interface IClient : IAddressable, IAsyncDisposable, IServiceProvider, ISigner
{
    event EventHandler? Attached;

    event EventHandler? Detached;

    event EventHandler? Started;

    event EventHandler? Stopped;

    event EventHandler? Disposed;

    bool IsAttached { get; }

    bool IsRunning { get; }

    AppEndPoint EndPoint { get; }

    ClientInfo Info { get; }

    AppPublicKey PublicKey { get; }

    Task AttachAsync(CancellationToken cancellationToken);

    Task DetachAsync(CancellationToken cancellationToken);

    Task StartAsync(INode node, CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);

    Task<AppId> SendTransactionAsync(string text, CancellationToken cancellationToken);
}
