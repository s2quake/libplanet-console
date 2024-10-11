using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Node.Commands;
using LibplanetConsole.Seed;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Node;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNode(
        this IServiceCollection @this, ApplicationOptions options)
    {
        var synchronizationContext = SynchronizationContext.Current ?? new();
        SynchronizationContext.SetSynchronizationContext(synchronizationContext);
        @this.AddSingleton(synchronizationContext);
        @this.AddSingleton(options);
        @this.AddSingleton<SeedService>()
             .AddSingleton<ISeedService>(s => s.GetRequiredService<SeedService>());
        @this.AddSingleton<Node>()
             .AddSingleton<INode>(s => s.GetRequiredService<Node>())
             .AddSingleton<IBlockChain>(s => s.GetRequiredService<Node>());
        @this.AddHostedService<NodeHostedService>();
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
