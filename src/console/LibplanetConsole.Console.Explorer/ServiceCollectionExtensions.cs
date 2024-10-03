using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Console.Explorer.Commands;
using LibplanetConsole.Console.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Explorer;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExplorer(this IServiceCollection @this)
    {
        @this.AddScoped<Explorer>()
             .AddScoped<IExplorer>(s => s.GetRequiredService<Explorer>())
             .AddScoped<INodeContent>(s => s.GetRequiredService<Explorer>())
             .AddScoped<INodeContentService>(s => s.GetRequiredService<Explorer>());

        @this.AddSingleton<IInfoProvider, ExplorerInfoProvider>();

        @this.AddSingleton<ICommand, ExplorerCommand>();

        return @this;
    }
}
