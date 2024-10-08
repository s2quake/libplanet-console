#if LIBPLANET_CONSOLE
namespace LibplanetConsole.Console.Grpc;
#elif LIBPLANET_NODE
namespace LibplanetConsole.Node.Grpc;
#elif LIBPLANET_CLIENT
namespace LibplanetConsole.Client.Grpc;
#else
#error "Either LIBPLANET_CONSOLE or LIBPLANET_NODE must be defined."
#endif

internal abstract class RunTask : IDisposable
{
    private bool _isRunning;
    private bool _disposedValue;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task _runningTask = Task.CompletedTask;

    public bool IsRunning => _isRunning;

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_isRunning is true)
        {
            throw new InvalidOperationException(
                $"{GetType().Name} is already running.");
        }

        _cancellationTokenSource = new();
        _runningTask = OnRunAsync(_cancellationTokenSource.Token);
        _isRunning = true;
        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_isRunning is false)
        {
            throw new InvalidOperationException(
                $"{GetType().Name} is not running.");
        }

        if (_cancellationTokenSource is not null)
        {
            await _cancellationTokenSource.CancelAsync();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }

        await _runningTask;
        _isRunning = false;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken);
        await StartAsync(cancellationTokenSource.Token);
        await _runningTask;
        await StopAsync(cancellationTokenSource.Token);
    }

    protected abstract Task OnRunAsync(CancellationToken cancellationToken);

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }

            _disposedValue = true;
        }
    }
}
