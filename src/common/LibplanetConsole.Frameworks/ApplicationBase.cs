namespace LibplanetConsole.Frameworks;

public abstract class ApplicationBase : IAsyncDisposable, IServiceProvider
{
    private readonly ManualResetEvent _closeEvent = new(false);
    private readonly SynchronizationContext _synchronizationContext;
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _isDisposed;

    protected ApplicationBase()
    {
        SynchronizationContext.SetSynchronizationContext(new());
        _synchronizationContext = SynchronizationContext.Current!;
    }

    public virtual ApplicationServiceCollection ApplicationServices { get; } = new([]);

    protected virtual bool CanClose => false;

    public Task InvokeAsync(Action action)
    {
        return Task.Run(() => _synchronizationContext.Send((state) => action(), null));
    }

    public Task<T> InvokeAsync<T>(Func<T> func)
    {
        return Task.Run(() =>
        {
            T result = default!;
            _synchronizationContext.Send((state) => result = func(), null);
            return result;
        });
    }

    public void Cancel()
    {
        if (_isDisposed == true)
        {
            throw new ObjectDisposedException($"{this}");
        }

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = null;
        _closeEvent.Set();
    }

    public async Task StartAsync()
    {
        if (_isDisposed == true)
        {
            throw new ObjectDisposedException($"{this}");
        }

        _cancellationTokenSource = new();
        await OnStartAsync(_cancellationTokenSource.Token);
        await Task.Run(() =>
        {
            while (CanClose != true && _closeEvent.WaitOne(1) != true)
            {
            }
        });
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed == true)
        {
            throw new ObjectDisposedException($"{this}");
        }

        await OnDisposeAsync();
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }

    public abstract object? GetService(Type serviceType);

    protected virtual async Task OnStartAsync(CancellationToken cancellationToken)
    {
        await ApplicationServices.InitializeAsync(this, cancellationToken: default);
    }

    protected virtual async ValueTask OnDisposeAsync()
    {
        await ApplicationServices.DisposeAsync();
    }
}
