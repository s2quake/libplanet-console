using Serilog;

namespace LibplanetConsole.Frameworks;

public abstract class ApplicationFramework : IAsyncDisposable, IServiceProvider
{
    private readonly ManualResetEvent _closeEvent = new(false);
    private readonly SynchronizationContext _synchronizationContext;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private bool _isDisposed;

    protected ApplicationFramework()
    {
        SynchronizationContext.SetSynchronizationContext(new());
        _synchronizationContext = SynchronizationContext.Current!;
        ApplicationLogger.SetApplication(this);
    }

    public virtual ApplicationServiceCollection ApplicationServices { get; } = new([]);

    public abstract ILogger Logger { get; }

    protected virtual bool CanClose => false;

    public Task InvokeAsync(Action action, CancellationToken cancellationToken)
    {
        return Task.Run(Action, cancellationToken);

        void Action() => _synchronizationContext.Send((state) => action(), null);
    }

    public Task<T> InvokeAsync<T>(Func<T> func, CancellationToken cancellationToken)
    {
        return Task.Run(Func, cancellationToken);

        T Func()
        {
            T result = default!;
            _synchronizationContext.Send((state) => result = func(), null);
            return result;
        }
    }

    public void Cancel()
    {
        Logger.Debug("Application canceled.");
        _cancellationTokenSource.Cancel();
        _closeEvent.Set();
    }

    public async Task RunAsync()
    {
        ObjectDisposedException.ThrowIf(_isDisposed == true, this);

        Logger.Debug("Application running...");
        await OnRunAsync(_cancellationTokenSource.Token);
        Logger.Debug("Application waiting for close...");
        await Task.Run(() =>
        {
            while (CanClose != true && _closeEvent.WaitOne(1) != true)
            {
                // Wait for close.
            }
        });
        Logger.Debug("Application run completed.");
    }

    public async ValueTask DisposeAsync()
    {
        ObjectDisposedException.ThrowIf(_isDisposed == true, this);

        Logger.Debug("Application disposing...");
        await OnDisposeAsync();
        _cancellationTokenSource.Dispose();
        _isDisposed = true;
        GC.SuppressFinalize(this);
        Logger.Debug("Application disposed.");
    }

    public abstract object? GetService(Type serviceType);

    protected virtual Task OnRunAsync(CancellationToken cancellationToken)
        => ApplicationServices.InitializeAsync(this, cancellationToken: default);

    protected virtual ValueTask OnDisposeAsync() => ValueTask.CompletedTask;
}
