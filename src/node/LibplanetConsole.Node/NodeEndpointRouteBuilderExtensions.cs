using LibplanetConsole.Node.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace LibplanetConsole.Node;

public static class NodeEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder UseNode(this IEndpointRouteBuilder @this)
    {
        @this.MapGrpcService<NodeGrpcServiceV1>();
        @this.MapGrpcService<BlockChainGrpcServiceV1>();
        @this.MapGrpcService<HubGrpcServiceV1>();

        return @this;
    }
}
