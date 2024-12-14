using JSSoft.Commands;
using Libplanet.Explorer;
using LibplanetConsole.Common;
using LibplanetConsole.Node.Explorer.Commands;
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
        @this.AddSingleton<Explorer>()
            .AddSingleton<IExplorer>(s => s.GetRequiredService<Explorer>())
            .AddSingleton<INodeContent>(s => s.GetRequiredService<Explorer>());
        @this.AddSingleton<IInfoProvider, ExplorerInfoProvider>();

        @this.AddSingleton<ICommand, ExplorerCommand>();
        return @this;
    }
}
