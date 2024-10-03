using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Node.Example.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Node.Example;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExample(this IServiceCollection @this)
    {
        @this.AddSingleton<ExampleNode>()
             .AddSingleton<IExampleNode>(s => s.GetRequiredService<ExampleNode>());
        @this.AddSingleton<IInfoProvider, ExampleNodeInfoProvider>();
        @this.AddSingleton<ILocalService, ExampleNodeService>();
        @this.AddSingleton<ICommand, ExampleNodeCommand>();
        return @this;
    }
}
