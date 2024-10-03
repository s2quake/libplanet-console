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
        this IServiceCollection services, ApplicationOptions options)
        where T : ApplicationBase
    {
        services.AddSingleton(
            ApplicationFramework.CreateLogger(typeof(T), options.LogPath, string.Empty));
        services.AddSingletonWithInterfaces<IApplication, IServiceProvider, T>();
        services.AddSingletonWithInterfaces<INodeCollection, IApplicationService, NodeCollection>();
        services.AddSingletonWithInterfaces<
            IClientCollection, IApplicationService, ClientCollection>();

        services.AddSingleton<ConsoleServiceContext>();
        services.AddSingletonWithInterfaces<ILocalService, IApplicationService, SeedService>();

        services.AddSingletonWithInterface<ICommand, ClientCommand>();
        services.AddSingletonWithInterface<ICommand, ExitCommand>();
        services.AddSingletonWithInterface<ICommand, InfoCommand>();
        services.AddSingletonWithInterface<ICommand, KeyCommand>();
        services.AddSingletonWithInterface<ICommand, NodeCommand>();
        services.AddSingletonWithInterface<ICommand, TxCommand>();
        return services;
    }
}
