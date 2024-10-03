using JSSoft.Commands;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Console.Commands;
using LibplanetConsole.Console.Services;
using LibplanetConsole.Framework;
using LibplanetConsole.Framework.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConsoleApplication<T>(
        this IServiceCollection @this, ApplicationOptions options)
        where T : ApplicationBase
    {
        @this.AddSingleton(
            ApplicationFramework.CreateLogger(typeof(T), options.LogPath, string.Empty));
        @this.AddSingletonWithInterfaces<IApplication, IServiceProvider, T>();
        @this.AddSingletonWithInterfaces<INodeCollection, IApplicationService, NodeCollection>();
        @this.AddSingletonWithInterfaces<
            IClientCollection, IApplicationService, ClientCollection>();

        @this.AddSingleton<ConsoleServiceContext>();
        @this.AddSingletonWithInterfaces<ILocalService, IApplicationService, SeedService>();

        @this.AddSingletonWithInterface<ICommand, ClientCommand>();
        @this.AddSingletonWithInterface<ICommand, ExitCommand>();
        @this.AddSingletonWithInterface<ICommand, InfoCommand>();
        @this.AddSingletonWithInterface<ICommand, KeyCommand>();
        @this.AddSingletonWithInterface<ICommand, NodeCommand>();
        @this.AddSingletonWithInterface<ICommand, TxCommand>();
        return @this;
    }
}
