using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using LibplanetConsole.Common;

namespace LibplanetConsole.Evidence.Grpc;

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
