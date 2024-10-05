using Libplanet.Explorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Node.Explorer;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExplorer(
        this IServiceCollection @this, IConfiguration configuration)
    {
        var startup = new ExplorerStartup<BlockChainContext>(configuration);
        startup.ConfigureServices(@this);
        @this.AddEndpointsApiExplorer();
        @this.AddSingleton<BlockChainContext>();
        @this.AddSingleton(startup);

        return @this;
    }
}
