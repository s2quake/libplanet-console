using JSSoft.Commands;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Framework;
using LibplanetConsole.Framework.Extensions;
using LibplanetConsole.Node.Commands;
using LibplanetConsole.Node.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Node;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNodeApplication<T>(
        this IServiceCollection services, ApplicationOptions options)
        where T : ApplicationBase
    {
        services.AddSingleton(
            ApplicationFramework.CreateLogger(typeof(T), options.LogPath, options.LibraryLogPath));
        services.AddSingletonWithInterfaces<IApplication, IServiceProvider, T>();
        services.AddSingletonWithInterfaces<INode, IBlockChain, Node>();
        services.AddSingleton<NodeContext>();

        services.AddSingletonWithInterfaces<ILocalService, IApplicationService, SeedService>();

        services.AddSingletonWithInterface<ILocalService, BlockChainService>();
        services.AddSingletonWithInterface<ILocalService, NodeService>();

        services.AddSingletonWithInterface<ICommand, AddressCommand>();
        services.AddSingletonWithInterface<ICommand, ExitCommand>();
        services.AddSingletonWithInterface<ICommand, InfoCommand>();
        services.AddSingletonWithInterface<ICommand, KeyCommand>();
        services.AddSingletonWithInterface<ICommand, StartCommand>();
        services.AddSingletonWithInterface<ICommand, TxCommand>();
        return services;
    }
}
