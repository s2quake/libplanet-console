using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Node.Explorer.Commands;
using Microsoft.Extensions.Configuration;

namespace LibplanetConsole.Node.Explorer;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExplorer(
        this IServiceCollection @this, IConfiguration configuration)
    {
        @this.AddEndpointsApiExplorer();
        @this.AddSingleton<Explorer>()
            .AddSingleton<IExplorer>(s => s.GetRequiredService<Explorer>())
            .AddSingleton<INodeContent>(s => s.GetRequiredService<Explorer>());
        @this.AddSingleton<IInfoProvider, ExplorerInfoProvider>();

        @this.AddSingleton<ICommand, ExplorerCommand>();
        return @this;
    }
}
