using LibplanetConsole.Client.Guild.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace LibplanetConsole.Client.Guild;

public static class NodeEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder UseGuild(this IEndpointRouteBuilder @this)
    {
        @this.MapGrpcService<GuildServiceGrpcV1>();

        return @this;
    }
}
