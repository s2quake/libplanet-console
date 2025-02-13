using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace LibplanetConsole.Node.Explorer;

public static class NodeEndpointRouteBuilderExtensions
{
    public static IApplicationBuilder UseExplorer(this IApplicationBuilder @this)
    {
        return @this;
    }
}
