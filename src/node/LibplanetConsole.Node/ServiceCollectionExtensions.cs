using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Framework;
using LibplanetConsole.Node.Commands;
using LibplanetConsole.Node.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Node;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNode(
        this IServiceCollection @this, ApplicationOptions options)
    {
        @this.AddSingleton(s => new Node(s, options))
             .AddSingleton<INode>(s => s.GetRequiredService<Node>())
             .AddSingleton<IBlockChain>(s => s.GetRequiredService<Node>());
        // @this.AddSingleton<NodeContext>();
        // @this.AddSingleton<SeedService>()
            //  .AddSingleton<ILocalService>(s => s.GetRequiredService<SeedService>())
            //  .AddSingleton<IApplicationService>(s => s.GetRequiredService<SeedService>());
        // @this.AddSingleton<ILocalService, BlockChainService>();
        // @this.AddSingleton<ILocalService, NodeService>();
        @this.AddSingleton<IInfoProvider, ApplicationInfoProvider>();
        @this.AddSingleton<IInfoProvider, NodeInfoProvider>();

        @this.AddSingleton<ICommand, AddressCommand>();
        @this.AddSingleton<ICommand, ExitCommand>();
        @this.AddSingleton<ICommand, InfoCommand>();
        @this.AddSingleton<ICommand, KeyCommand>();
        @this.AddSingleton<ICommand, StartCommand>();
        @this.AddSingleton<ICommand, StopCommand>();
        @this.AddSingleton<ICommand, TxCommand>();
        return @this;
    }
}
