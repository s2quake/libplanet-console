#if LIBPLANET_NODE || LIBPLANET_CLIENT
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using LibplanetConsole.Common;

namespace LibplanetConsole.Hub.Services;

internal static class HubChannel
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
                            Service = "libplanet.console.hub.v1.HubGrpcService",
                            Method = "GetService",
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

    public static GrpcChannel CreateChannel(Uri url)
    {
        var address = url.ToString();
        return GrpcChannel.ForAddress(address, _channelOptions);
    }

    public static GrpcChannel CreateChannel(string address)
    {
        return GrpcChannel.ForAddress(address, _channelOptions);
    }
}
#endif // LIBPLANET_NODE || LIBPLANET_CLIENT
