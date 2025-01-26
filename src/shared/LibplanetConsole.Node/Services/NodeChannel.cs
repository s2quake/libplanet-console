#if LIBPLANET_CLIENT || LIBPLANET_CONSOLE
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using LibplanetConsole.Common;
using static LibplanetConsole.Grpc.Node.NodeGrpcService;

#if LIBPLANET_CLIENT
namespace LibplanetConsole.Client.Services;
#else
namespace LibplanetConsole.Console.Services;
#endif

internal static class NodeChannel
{
    private static readonly GrpcChannelOptions _channelOptions = new()
    {
        ThrowOperationCanceledOnCancellation = true,
        MaxRetryAttempts = 10,
        ServiceConfig = new()
        {
            MethodConfigs =
            {
                new MethodConfig
                {
                    Names =
                    {
                        new MethodName
                        {
                            Service = "libplanet.console.node.v1.NodeGrpcService",
                            Method = "Ping",
                        },
                    },
                    RetryPolicy = new RetryPolicy
                    {
                        MaxAttempts = 5,
                        InitialBackoff = TimeSpan.FromSeconds(1),
                        MaxBackoff = TimeSpan.FromSeconds(5),
                        BackoffMultiplier = 1.5,
                        RetryableStatusCodes =
                        {
                            StatusCode.Unavailable,
                        },
                    },
                },
            },
        },
    };

    public static GrpcChannel CreateChannel(EndPoint endPoint)
    {
        var address = $"http://{EndPointUtility.ToString(endPoint)}";
        return GrpcChannel.ForAddress(address, _channelOptions);
    }
}
#endif // LIBPLANET_CLIENT || LIBPLANET_CONSOLE
