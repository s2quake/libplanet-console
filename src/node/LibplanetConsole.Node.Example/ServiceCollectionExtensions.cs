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
        @this.AddSingleton<Example>()
             .AddSingleton<IExample>(s => s.GetRequiredService<Example>());
        @this.AddSingleton<IInfoProvider, ExampleInfoProvider>();
        @this.AddSingleton<ILocalService, ExampleService>();
        @this.AddSingleton<ICommand, ExampleCommand>();
        return @this;
    }
}
