using LibplanetConsole.Client;
using LibplanetConsole.Common;

namespace LibplanetConsole.Console;

public interface IClient : IAddressable, IAsyncDisposable, IServiceProvider, ISigner
{
    const string Key = nameof(IClient);

    event EventHandler? Attached;

    event EventHandler? Detached;

    event EventHandler? Started;

    event EventHandler? Stopped;

    event EventHandler? Disposed;

    bool IsAttached { get; }

    bool IsRunning { get; }

    EndPoint EndPoint { get; }

    ClientInfo Info { get; }

    PublicKey PublicKey { get; }

    Task AttachAsync(CancellationToken cancellationToken);

    Task DetachAsync(CancellationToken cancellationToken);

    Task StartAsync(INode node, CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
