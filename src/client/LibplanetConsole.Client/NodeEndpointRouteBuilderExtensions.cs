using LibplanetConsole.Client.Grpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace LibplanetConsole.Client;

public static class NodeEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder UseClient(this IEndpointRouteBuilder @this)
    {
        @this.MapGrpcService<ClientGrpcServiceV1>();
        @this.MapGrpcService<BlockChainGrpcServiceV1>();

        return @this;
    }
}
