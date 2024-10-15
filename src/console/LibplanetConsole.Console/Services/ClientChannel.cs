using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using LibplanetConsole.Common;

namespace LibplanetConsole.Console.Services;

internal static class ClientChannel
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
                            Service = "libplanet.console.client.v1.ClientGrpcService",
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
