#if LIBPLANET_NODE || LIBPLANET_CLIENT
using Grpc.Core;
using Grpc.Net.Client;
using LibplanetConsole.Hub.Grpc;
using static LibplanetConsole.Hub.Grpc.HubGrpcService;

namespace LibplanetConsole.Hub.Services;

internal sealed class HubService(GrpcChannel channel)
    : HubGrpcServiceClient(channel)
{
    public static async Task<Uri> GetServiceUrlAsync(
        Uri hubUrl, string serviceName, CancellationToken cancellationToken)
    {
        using var channel = HubChannel.CreateChannel(hubUrl);
        var service = new HubService(channel);
        var request = new GetServiceRequest
        {
            ServiceName = serviceName,
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await service.GetServiceAsync(request, callOptions);
        return new Uri(response.Url);
    }
}
#endif // LIBPLANET_NODE || LIBPLANET_CLIENT
