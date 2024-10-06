using Grpc.Core;

namespace LibplanetConsole.Node.Services;

internal abstract class Callback<T>(IAsyncStreamWriter<T> streamWriter)
{
    private CancellationTokenSource? _cancellationTokenSource;

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        if (_cancellationTokenSource is not null)
        {
            throw new InvalidOperationException("Callback is already running.");
        }

        using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken);

        _cancellationTokenSource = cancellationTokenSource;
        try
        {
            await OnRun(cancellationTokenSource.Token);
        }
        finally
        {
            _cancellationTokenSource = null;
        }
    }

    protected virtual async Task OnRun(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested is false)
        {
            await Task.Delay(1, default);
        }
    }

    protected async Task InvokeAsync(T value)
    {
        if (_cancellationTokenSource is null)
        {
            throw new InvalidOperationException("Callback is not running.");
        }

        await streamWriter.WriteAsync(value, _cancellationTokenSource.Token);
    }
}
