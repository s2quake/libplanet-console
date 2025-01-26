using LibplanetConsole.Common;

namespace LibplanetConsole.Console;

public interface IClient : IAsyncDisposable, IKeyedServiceProvider, ISigner
{
    const string Key = nameof(IClient);

    const string Tag = "client";

    event EventHandler? Attached;

    event EventHandler? Detached;

    event EventHandler? Started;

    event EventHandler? Stopped;

    event EventHandler? Disposed;

    int ProcessId { get; }

    bool IsAttached { get; }

    bool IsRunning { get; }

    EndPoint EndPoint { get; set; }

    ClientInfo Info { get; }

    PublicKey PublicKey { get; }

    Address Address => PublicKey.Address;

    string Alias { get; }

    Task StartProcessAsync(ProcessOptions options, CancellationToken cancellationToken);

    Task StopProcessAsync(CancellationToken cancellationToken);

    Task AttachAsync(CancellationToken cancellationToken);

    Task DetachAsync(CancellationToken cancellationToken);

    Task StartAsync(INode node, CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);

    string GetCommandLine();

    Task<TxId> SendTransactionAsync(IAction[] actions, CancellationToken cancellationToken);
}
