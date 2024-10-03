using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Framework.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Client.Example;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExample(this IServiceCollection @this)
    {
        @this.AddSingletonWithInterface<IExampleClient, ExampleClient>();
        @this.AddSingleton<ICommand, ExampleClientCommand>();
        @this.AddSingleton<IInfoProvider, ExampleClientInfoProvider>();
        @this.AddSingleton<ILocalService, ExampleClientService>();
        @this.AddSingleton<ExampleRemoteNodeService>();
        return @this;
    }
}
