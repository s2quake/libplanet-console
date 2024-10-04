using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LibplanetConsole.Framework;

public abstract class ApplicationFramework : IAsyncDisposable, IServiceProvider
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ManualResetEvent _closeEvent = new(false);
    private readonly SynchronizationContext _synchronizationContext;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly ILogger<ApplicationFramework> _logger;
    private bool _isDisposed;

    protected ApplicationFramework(IServiceProvider serviceProvider)
    {
        SynchronizationContext.SetSynchronizationContext(new());
        _logger = serviceProvider.GetRequiredService<ILogger<ApplicationFramework>>();
        _serviceProvider = serviceProvider;
        _synchronizationContext = SynchronizationContext.Current!;
    }

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
        _logger.LogDebug("Application canceled.");
        _cancellationTokenSource.Cancel();
        _closeEvent.Set();
    }

    public async Task RunAsync()
    {
        ObjectDisposedException.ThrowIf(_isDisposed == true, this);

        _logger.LogDebug("Application running...");
        await OnRunAsync(_cancellationTokenSource.Token);
        _logger.LogDebug("Application waiting for close...");
        await Task.Run(() =>
        {
            while (CanClose != true && _closeEvent.WaitOne(1) != true)
            {
                // Wait for close.
            }
        });
        _logger.LogDebug("Application run completed.");
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed is false)
        {
            _logger.LogDebug("Application disposing...");
            await OnDisposeAsync();
            _cancellationTokenSource.Dispose();
            _isDisposed = true;
            GC.SuppressFinalize(this);
            _logger.LogDebug("Application disposed.");
        }
    }

    public abstract object? GetService(Type serviceType);

    protected virtual async Task OnRunAsync(CancellationToken cancellationToken)
    {
        var applicationServices = Sort(_serviceProvider.GetServices<IApplicationService>());

        for (var i = 0; i < applicationServices.Length; i++)
        {
            var serviceName = applicationServices[i].GetType().Name;
            _logger.LogDebug("Application service initializing: {ServiceName}", serviceName);
            await applicationServices[i].InitializeAsync(cancellationToken);
            _logger.LogDebug("Application service initialized: {ServiceName}", serviceName);
        }
    }

    protected virtual ValueTask OnDisposeAsync() => ValueTask.CompletedTask;

    private static IApplicationService[] Sort(IEnumerable<IApplicationService> items)
    {
        return DependencyUtility.TopologicalSort(items, GetDependencies).ToArray();

        IEnumerable<IApplicationService> GetDependencies(IApplicationService item)
        {
            return DependencyUtility.GetDependencies(item, items);
        }
    }
}
