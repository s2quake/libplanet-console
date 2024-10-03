using JSSoft.Commands;
using LibplanetConsole.Client.Commands;
using LibplanetConsole.Client.Services;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Framework;
using LibplanetConsole.Framework.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Client;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddClientApplication<T>(
        this IServiceCollection @this, ApplicationOptions options)
        where T : ApplicationBase
    {
        @this.AddSingleton(
            ApplicationFramework.CreateLogger(typeof(T), options.LogPath, string.Empty));
        @this.AddSingletonWithInterfaces<IApplication, IServiceProvider, T>();
        @this.AddSingletonWithInterfaces<IClient, IBlockChain, Client>();

        @this.AddSingletonWithInterfaces<ILocalService, IClientService, ClientService>();
        @this.AddSingleton<ClientServiceContext>();
        @this.AddSingletonWithInterface<IRemoteService, RemoteBlockChainService>();
        @this.AddSingleton<RemoteNodeContext>();
        @this.AddSingletonWithInterface<IRemoteService, RemoteNodeService>();

        @this.AddSingletonWithInterface<ICommand, AddressCommand>();
        @this.AddSingletonWithInterface<ICommand, ExitCommand>();
        @this.AddSingletonWithInterface<ICommand, InfoCommand>();
        @this.AddSingletonWithInterface<ICommand, KeyCommand>();
        @this.AddSingletonWithInterface<ICommand, StartCommand>();
        @this.AddSingletonWithInterface<ICommand, StopCommand>();
        @this.AddSingletonWithInterface<ICommand, TxCommand>();
        return @this;
    }
}
