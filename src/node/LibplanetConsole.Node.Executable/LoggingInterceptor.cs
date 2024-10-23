#pragma warning disable S2139 // Exceptions should be either logged or rethrown but not both
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace LibplanetConsole.Node.Executable;

internal class LoggingInterceptor(ILogger<LoggingInterceptor> logger) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while processing gRPC request.");
            throw;
        }
    }
}
