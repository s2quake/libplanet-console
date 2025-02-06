using LibplanetConsole.Node.Guild.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace LibplanetConsole.Node.Guild;

public static class NodeEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder UseGuild(this IEndpointRouteBuilder @this)
    {
        @this.MapGrpcService<GuildServiceGrpcV1>();

        return @this;
    }
}
