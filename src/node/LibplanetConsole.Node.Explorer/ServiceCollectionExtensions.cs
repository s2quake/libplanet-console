using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Framework;
using LibplanetConsole.Node.Explorer.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Node.Explorer;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExplorer(this IServiceCollection @this)
    {
        @this.AddSingleton<Explorer>()
             .AddSingleton<IExplorer>(s => s.GetRequiredService<Explorer>())
             .AddSingleton<IApplicationService>(s => s.GetRequiredService<Explorer>());
        @this.AddSingleton<IInfoProvider, ExplorerInfoProvider>();
        // @this.AddSingleton<ILocalService, ExplorerService>();
        @this.AddSingleton<ICommand, ExplorerCommand>();
        return @this;
    }
}
