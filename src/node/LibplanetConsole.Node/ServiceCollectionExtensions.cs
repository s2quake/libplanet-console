using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Framework;
using LibplanetConsole.Node.Commands;
using LibplanetConsole.Node.Services;
using LibplanetConsole.Seed;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Node;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNode(
        this IServiceCollection @this, ApplicationOptions options)
    {
        @this.AddSingleton<SeedService>()
             .AddSingleton<ISeedService>(s => s.GetRequiredService<SeedService>())
             .AddHostedService(s => s.GetRequiredService<SeedService>());
        @this.AddSingleton(s => new Node(s, options))
             .AddSingleton<INode>(s => s.GetRequiredService<Node>())
             .AddSingleton<IBlockChain>(s => s.GetRequiredService<Node>());
        @this.AddHostedService<NodeHostedService>();
        // @this.AddSingleton<NodeContext>();
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
