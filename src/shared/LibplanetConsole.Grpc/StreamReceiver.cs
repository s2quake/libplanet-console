#pragma warning disable SA1402 // File may only contain a single type
using Grpc.Core;

#if LIBPLANET_CONSOLE
namespace LibplanetConsole.Console.Grpc;
#elif LIBPLANET_NODE
namespace LibplanetConsole.Node.Grpc;
#elif LIBPLANET_CLIENT
namespace LibplanetConsole.Client.Grpc;
#else
#error "Either LIBPLANET_CONSOLE or LIBPLANET_NODE must be defined."
#endif

internal sealed class StreamReceiver<TResponse>(
    AsyncServerStreamingCall<TResponse> streamingCall,
    Action<TResponse> action) : RunTask
{
    protected override async Task OnRunAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (await streamingCall.ResponseStream.MoveNext(cancellationToken))
            {
                var response = streamingCall.ResponseStream.Current;
                action.Invoke(response);
            }
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
        {
            // Ignore
        }
        catch (OperationCanceledException)
        {
            // Ignore
        }
    }
}
