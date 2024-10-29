using Grpc.Core;

namespace LibplanetConsole.Grpc;

internal sealed class StreamReceiver<TResponse>(
    Func<AsyncServerStreamingCall<TResponse>> streamingCallFunc,
    Action<TResponse> action)
    : RunTaskBase
{
    public event EventHandler? Aborted;

    public event EventHandler? Completed;

    protected override async Task OnRunAsync(CancellationToken cancellationToken)
    {
        var streamingCall = streamingCallFunc();
        try
        {
            while (await streamingCall.ResponseStream.MoveNext(cancellationToken))
            {
                var response = streamingCall.ResponseStream.Current;
                action.Invoke(response);
            }

            Completed?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception)
        {
            Aborted?.Invoke(this, EventArgs.Empty);
        }
    }
}
