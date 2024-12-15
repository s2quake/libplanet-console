using Lib9c;
using LibplanetConsole.Node.Bank;
using LibplanetConsole.Node.Guild.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Node.Guild;

public static class NodeEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder UseGuild(this IEndpointRouteBuilder @this)
    {
        @this.MapGrpcService<GuildServiceGrpcV1>();

        return @this;
    }
}
