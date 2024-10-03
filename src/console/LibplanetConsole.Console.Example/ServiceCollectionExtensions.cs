using JSSoft.Commands;
using LibplanetConsole.Common;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Example;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExample(this IServiceCollection @this)
    {
        @this.AddScoped<ExampleNode>()
             .AddScoped<IExampleNode>(s => s.GetRequiredService<ExampleNode>());
        @this.AddScoped<ExampleClient>()
             .AddScoped<IExampleClient>(s => s.GetRequiredService<ExampleClient>());

        @this.AddSingleton<IInfoProvider, ExampleNodeInfoProvider>();
        @this.AddSingleton<IInfoProvider, ExampleClientInfoProvider>();

        @this.AddSingleton<ICommand, ExampleNodeCommand>();
        @this.AddSingleton<ICommand, ExampleClientCommand>();

        return @this;
    }
}
