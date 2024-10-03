using JSSoft.Commands;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Framework;
using LibplanetConsole.Framework.Extensions;
using LibplanetConsole.Client.Commands;
using LibplanetConsole.Client.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Client;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddClientApplication<T>(
        this IServiceCollection services, ApplicationOptions options)
        where T : ApplicationBase
    {
        services.AddSingleton(
            ApplicationFramework.CreateLogger(typeof(T), options.LogPath, string.Empty));
        services.AddSingletonWithInterfaces<IApplication, IServiceProvider, T>();
        services.AddSingletonWithInterfaces<IClient, IBlockChain, Client>();

        services.AddSingletonWithInterfaces<ILocalService, IClientService, ClientService>();
        services.AddSingleton<ClientServiceContext>();
        services.AddSingletonWithInterface<IRemoteService, RemoteBlockChainService>();
        services.AddSingleton<RemoteNodeContext>();
        services.AddSingletonWithInterface<IRemoteService, RemoteNodeService>();

        services.AddSingletonWithInterface<ICommand, AddressCommand>();
        services.AddSingletonWithInterface<ICommand, ExitCommand>();
        services.AddSingletonWithInterface<ICommand, InfoCommand>();
        services.AddSingletonWithInterface<ICommand, KeyCommand>();
        services.AddSingletonWithInterface<ICommand, StartCommand>();
        services.AddSingletonWithInterface<ICommand, StopCommand>();
        services.AddSingletonWithInterface<ICommand, TxCommand>();
        return services;
    }
}
