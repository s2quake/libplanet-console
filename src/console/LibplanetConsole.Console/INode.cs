using LibplanetConsole.Common;
using LibplanetConsole.Node;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console;

public interface INode : IAddressable, IAsyncDisposable, IKeyedServiceProvider, ISigner
{
    const string Key = nameof(INode);

    event EventHandler? Attached;

    event EventHandler? Detached;

    event EventHandler? Started;

    event EventHandler? Stopped;

    event EventHandler? Disposed;

    bool IsAttached { get; }

    bool IsRunning { get; }

    EndPoint EndPoint { get; }

    NodeInfo Info { get; }

    PublicKey PublicKey { get; }

    Task AttachAsync(CancellationToken cancellationToken);

    Task DetachAsync(CancellationToken cancellationToken);

    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
