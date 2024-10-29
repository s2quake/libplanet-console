using LibplanetConsole.Common.Threading;

namespace LibplanetConsole.Grpc;

internal abstract class RunTaskBase : IDisposable
{
    private bool _isRunning;
    private bool _disposedValue;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task _runningTask = Task.CompletedTask;

    public bool IsRunning => _isRunning;

    public string Name { get; set; } = string.Empty;

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_isRunning is true)
        {
            throw new InvalidOperationException($"{GetType().Name} is already running.");
        }

        await OnStartAsync(cancellationToken);
        _cancellationTokenSource = new();
        _runningTask = OnRunAsync(_cancellationTokenSource.Token);
        _isRunning = true;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_isRunning is false)
        {
            throw new InvalidOperationException($"{GetType().Name} is not running.");
        }

        if (_cancellationTokenSource is not null)
        {
            await _cancellationTokenSource.CancelAsync();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }

        await TaskUtility.TryWait(_runningTask);
        await OnStopAsync(cancellationToken);
        _isRunning = false;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        if (_cancellationTokenSource is not null)
        {
            throw new InvalidOperationException($"{GetType().Name} is already running.");
        }

        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken);
        try
        {
            await OnStartAsync(_cancellationTokenSource.Token);
            _runningTask = OnRunAsync(_cancellationTokenSource.Token);
            _isRunning = true;
            await _runningTask;
            await TaskUtility.TryWait(_runningTask);
            await OnStopAsync(_cancellationTokenSource.Token);
        }
        finally
        {
            _isRunning = false;
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
    }

    protected abstract Task OnRunAsync(CancellationToken cancellationToken);

    protected virtual Task OnStartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    protected virtual Task OnStopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
                TaskUtility.TryWait(_runningTask).Wait();
            }

            _disposedValue = true;
        }
    }
}
