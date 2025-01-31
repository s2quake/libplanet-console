#if LIBPLANET_NODE
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using LibplanetConsole.Common;

namespace LibplanetConsole.Seed.Services;

internal static class SeedChannel
{
    private static readonly GrpcChannelOptions _channelOptions = new()
    {
        ThrowOperationCanceledOnCancellation = true,
        MaxRetryAttempts = 10,
        MaxReceiveMessageSize = 10 * 1024 * 1024, // 10 MB
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
                            Service = "libplanet.console.console.v1.ConsoleGrpcService",
                            Method = "GetNodeSettings",
                        },
                        new MethodName
                        {
                            Service = "libplanet.console.console.v1.ConsoleGrpcService",
                            Method = "GetClientSettings",
                        },
                        new MethodName
                        {
                            Service = "libplanet.console.console.v1.ConsoleGrpcService",
                            Method = "AttachNode",
                        },
                        new MethodName
                        {
                            Service = "libplanet.console.console.v1.ConsoleGrpcService",
                            Method = "AttachClient",
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
#endif // LIBPLANET_NODE
