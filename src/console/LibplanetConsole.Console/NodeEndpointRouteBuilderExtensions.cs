using LibplanetConsole.Console.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace LibplanetConsole.Console;

public static class NodeEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder UseConsole(this IEndpointRouteBuilder @this)
    {
        @this.MapGrpcService<SeedGrpcServiceV1>();
        @this.MapGrpcService<ConsoleGrpcServiceV1>();

        return @this;
    }
}
