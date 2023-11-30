using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using JSSoft.Library.Commands;

namespace OnBoarding.ConsoleHost;

sealed partial class Application : IAsyncDisposable, IServiceProvider
{
    private readonly CompositionContainer _container;
    private SwarmHostCollection? _swarmHosts;
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _isDisposed;
    private SystemTerminal? _terminal;

    public Application()
    {
        _container = new(new AssemblyCatalog(typeof(Application).Assembly));
        _container.ComposeExportedValue(this);
        InitializeService();
    }

    public void Cancel()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = null;
    }

    public async Task StartAsync(string[] args)
    {
        if (_terminal != null)
            throw new InvalidOperationException("Application has already been started.");

        if (args.Length > 0 && GetService<CommandContext>() is { } commandContext)
        {
            await commandContext.ExecuteAsync(args, cancellationToken: default, progress: new Progress<ProgressInfo>());
        }
        _swarmHosts = _container.GetExportedValue<SwarmHostCollection>();
        _cancellationTokenSource = new();
        _terminal = _container.GetExportedValue<SystemTerminal>()!;
        await _terminal!.StartAsync(_cancellationTokenSource.Token);
    }

    public async ValueTask DisposeAsync()
    {
        ObjectDisposedException.ThrowIf(condition: _isDisposed, this);

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = null;
        if (_swarmHosts != null)
        await _swarmHosts.DisposeAsync();
        _terminal = null;
        _container.Dispose();
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }

    public T? GetService<T>()
    {
        return _container.GetExportedValue<T>();
    }

    public object? GetService(Type serviceType)
    {
        return _container.GetExportedValue<object?>(AttributedModelServices.GetContractName(serviceType));
    }
}
