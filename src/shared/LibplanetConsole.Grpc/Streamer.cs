using Grpc.Core;
using Microsoft.Extensions.Hosting;

namespace LibplanetConsole.Grpc;

internal abstract class Streamer<T>(IAsyncStreamWriter<T> streamWriter)
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

    public async Task RunAsync(
        IHostApplicationLifetime applicationLifetime, CancellationToken cancellationToken)
    {
        using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken);
        applicationLifetime.ApplicationStopping.Register(cancellationTokenSource.Cancel);
        await RunAsync(cancellationTokenSource.Token);
    }

    protected virtual async Task OnRun(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested is false)
        {
            await Task.Delay(1, default);
        }
    }

    protected async void WriteValue(T value)
    {
        if (_cancellationTokenSource is null)
        {
            throw new InvalidOperationException("Callback is not running.");
        }

        await streamWriter.WriteAsync(value, _cancellationTokenSource.Token);
    }
}
