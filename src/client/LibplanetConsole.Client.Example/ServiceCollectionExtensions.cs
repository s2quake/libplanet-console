using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Client.Example;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExample(this IServiceCollection @this)
    {
        @this.AddSingleton<Example>()
             .AddSingleton<IExample>(s => s.GetRequiredService<Example>());
        @this.AddSingleton<ICommand, ExampleCommand>();
        @this.AddSingleton<IInfoProvider, ExampleInfoProvider>();
        @this.AddSingleton<ILocalService, ExampleService>();
        @this.AddSingleton<ExampleRemoteService>();
        return @this;
    }
}
