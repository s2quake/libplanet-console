using Grpc.Core;
using LibplanetConsole.Hub.Grpc;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace LibplanetConsole.Node.Services;

internal sealed class HubGrpcServiceV1(IServer server)
    : HubGrpcService.HubGrpcServiceBase
{
    public override Task<GetServiceResponse> GetService(
        GetServiceRequest request, ServerCallContext context)
    {
        var serviceName = request.ServiceName;
        var url = GetUrl(serviceName);

        return Task.FromResult(new GetServiceResponse
        {
            Url = url,
        });
    }

    private string GetUrl(string serviceName)
    {
        var address = string.Empty;
        if (server.Features.Get<IServerAddressesFeature>() is { } addressesFeature)
        {
            address = addressesFeature.Addresses.First();
        }

        if (serviceName == "libplanet.console.node.v1")
        {
            return address;
        }

        throw new RpcException(new Status(StatusCode.NotFound, $"Service {serviceName} not found"));
    }
}
