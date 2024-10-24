using Grpc.Net.Client;
using LibplanetConsole.Common;

namespace LibplanetConsole.Evidence.Services;

internal static class EvidenceChannel
{
    private static readonly GrpcChannelOptions _channelOptions = new()
    {
        ThrowOperationCanceledOnCancellation = true,
        MaxRetryAttempts = 10,
        ServiceConfig = new()
        {
        },
    };

    public static GrpcChannel CreateChannel(EndPoint endPoint)
    {
        var address = $"http://{EndPointUtility.ToString(endPoint)}";
        return GrpcChannel.ForAddress(address, _channelOptions);
    }
}
