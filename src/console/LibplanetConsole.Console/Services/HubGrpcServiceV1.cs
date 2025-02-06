using Grpc.Core;
using LibplanetConsole.Hub.Grpc;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace LibplanetConsole.Console.Services;

internal sealed class HubGrpcServiceV1(INodeCollection nodes, IServer server)
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
            return RandomNodeUrl();
        }

        if (serviceName == "libplanet.console.seed.v1")
        {
            return address;
        }

        if (serviceName == "libplanet.console.alias.v1")
        {
            return address;
        }

        throw new RpcException(new Status(StatusCode.NotFound, $"Service {serviceName} not found"));
    }

    private string RandomNodeUrl()
    {
        var nodeIndex = Random.Shared.Next(nodes.Count);
        var node = nodes[nodeIndex];
        return node.Url.ToString();
    }
}
