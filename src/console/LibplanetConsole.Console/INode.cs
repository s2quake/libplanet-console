using LibplanetConsole.Common;
using LibplanetConsole.Node;

namespace LibplanetConsole.Console;

public interface INode : IAddressable, IAsyncDisposable, IServiceProvider, ISigner
{
    event EventHandler? Attached;

    event EventHandler? Detached;

    event EventHandler? Started;

    event EventHandler? Stopped;

    event EventHandler? Disposed;

    bool IsAttached { get; }

    bool IsRunning { get; }

    AppEndPoint EndPoint { get; }

    NodeInfo Info { get; }

    PublicKey PublicKey { get; }

    Task AttachAsync(CancellationToken cancellationToken);

    Task DetachAsync(CancellationToken cancellationToken);

    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
