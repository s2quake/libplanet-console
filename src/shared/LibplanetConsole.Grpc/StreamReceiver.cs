using Grpc.Core;

namespace LibplanetConsole.Grpc;

internal sealed class StreamReceiver<TResponse>(
    AsyncServerStreamingCall<TResponse> streamingCall,
    Action<TResponse> action) : RunTask
{
    public event EventHandler? Aborted;

    public event EventHandler? Completed;

    protected override async Task OnRunAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (await streamingCall.ResponseStream.MoveNext(cancellationToken))
            {
                var response = streamingCall.ResponseStream.Current;
                action.Invoke(response);
            }

            Completed?.Invoke(this, EventArgs.Empty);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
        {
            Aborted?.Invoke(this, EventArgs.Empty);
        }
        catch (OperationCanceledException)
        {
            // Ignore
        }
        catch (Exception ex)
        {
            int qwer = 0;
        }
    }
}
