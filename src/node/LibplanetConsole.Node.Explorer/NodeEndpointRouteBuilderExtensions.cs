using Libplanet.Explorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace LibplanetConsole.Node.Explorer;

public static class NodeEndpointRouteBuilderExtensions
{
    public static IApplicationBuilder UseExplorer(this IApplicationBuilder @this)
    {
        var serviceProvider = @this.ApplicationServices;
        var environment = serviceProvider.GetRequiredService<IWebHostEnvironment>();
        var startUp = serviceProvider.GetRequiredService<ExplorerStartup<BlockChainContext>>();
        startUp.Configure(@this, environment);

        return @this;
    }
}
