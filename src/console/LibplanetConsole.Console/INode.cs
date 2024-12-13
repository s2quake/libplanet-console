using LibplanetConsole.Common;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console;

public interface INode : IAsyncDisposable, IKeyedServiceProvider, ISigner
{
    const string Key = nameof(INode);

    event EventHandler? Attached;

    event EventHandler? Detached;

    event EventHandler? Started;

    event EventHandler? Stopped;

    event EventHandler? Disposed;

    int ProcessId { get; }

    bool IsAttached { get; }

    bool IsRunning { get; }

    EndPoint EndPoint { get; set; }

    NodeInfo Info { get; }

    PublicKey PublicKey { get; }

    Address Address => PublicKey.Address;

    Task StartProcessAsync(ProcessOptions options, CancellationToken cancellationToken);

    Task StopProcessAsync(CancellationToken cancellationToken);

    Task AttachAsync(CancellationToken cancellationToken);

    Task DetachAsync(CancellationToken cancellationToken);

    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);

    string GetCommandLine();

    Task<TxId> SendTransactionAsync(IAction[] actions, CancellationToken cancellationToken);
}
