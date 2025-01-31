using LibplanetConsole.Console.Seed.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace LibplanetConsole.Console.Seed;

public static class NodeEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder UseSeed(this IEndpointRouteBuilder @this)
    {
        @this.MapGrpcService<SeedGrpcServiceV1>();

        return @this;
    }
}
