using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Framework;
using LibplanetConsole.Framework.Extensions;
using LibplanetConsole.Node.Commands;
using LibplanetConsole.Node.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Node;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication<T>(
        this IServiceCollection @this, ApplicationOptions options)
        where T : ApplicationBase
    {
        @this.AddSingleton(
            ApplicationFramework.CreateLogger(typeof(T), options.LogPath, options.LibraryLogPath));
        @this.AddSingletonWithInterfaces<IApplication, IServiceProvider, T>();
        @this.AddSingletonWithInterfaces<INode, IBlockChain, Node>();
        @this.AddSingleton<NodeContext>();

        @this.AddSingletonWithInterfaces<ILocalService, IApplicationService, SeedService>();

        @this.AddSingletonWithInterface<ILocalService, BlockChainService>();
        @this.AddSingletonWithInterface<ILocalService, NodeService>();

        @this.AddSingletonWithInterface<IInfoProvider, ApplicationInfoProvider>();
        @this.AddSingletonWithInterface<IInfoProvider, NodeInfoProvider>();

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
