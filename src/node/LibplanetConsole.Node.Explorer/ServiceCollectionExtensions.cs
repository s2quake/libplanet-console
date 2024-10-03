using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Framework;
using LibplanetConsole.Framework.Extensions;
using LibplanetConsole.Node.Explorer.Commands;
using LibplanetConsole.Node.Explorer.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Node.Explorer;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExplorer(this IServiceCollection @this)
    {
        @this.AddSingletonWithInterfaces<IExplorer, IApplicationService, Explorer>();
        @this.AddSingleton<IInfoProvider, ExplorerInfoProvider>();
        @this.AddSingleton<ILocalService, ExplorerService>();
        @this.AddSingleton<ICommand, ExplorerCommand>();
        return @this;
    }
}
