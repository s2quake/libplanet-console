using LibplanetConsole.Node.Hand.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace LibplanetConsole.Node.Hand;

public static class NodeEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder UseHand(this IEndpointRouteBuilder @this)
    {
        @this.MapGrpcService<HandServiceGrpcV1>();

        return @this;
    }
}
