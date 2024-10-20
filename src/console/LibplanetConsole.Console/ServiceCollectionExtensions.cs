using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Console.Commands;
using LibplanetConsole.Seed;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConsole(
        this IServiceCollection @this, ApplicationOptions options)
    {
        var synchronizationContext = SynchronizationContext.Current ?? new();
        SynchronizationContext.SetSynchronizationContext(synchronizationContext);
        @this.AddSingleton(synchronizationContext);
        @this.AddSingleton(options);
        @this.AddSingleton<SeedService>()
             .AddSingleton<ISeedService>(s => s.GetRequiredService<SeedService>());
        @this.AddSingleton<NodeCollection>()
             .AddSingleton<INodeCollection>(s => s.GetRequiredService<NodeCollection>());
        @this.AddSingleton<ClientCollection>()
             .AddSingleton<IClientCollection>(s => s.GetRequiredService<ClientCollection>());
        @this.AddSingleton<BlockChain>()
             .AddSingleton<IBlockChain>(s => s.GetRequiredService<BlockChain>());

        @this.AddHostedService<ConsoleHostedService>();

        @this.AddKeyedScoped(INode.Key, NodeFactory.Create)
             .AddKeyedScoped<INode>(
                INode.Key, (s, k) => s.GetRequiredKeyedService<Node>(k))
             .AddKeyedScoped<IBlockChain>(
                INode.Key, (s, k) => s.GetRequiredKeyedService<Node>(k));
        @this.AddKeyedScoped(IClient.Key, ClientFactory.Create)
             .AddKeyedScoped<IClient>(
                IClient.Key, (s, k) => s.GetRequiredKeyedService<Client>(k))
             .AddKeyedScoped<IBlockChain>(
                IClient.Key, (s, k) => s.GetRequiredKeyedService<Client>(k));

        @this.AddSingleton<IInfoProvider, NodeInfoProvider>();
        @this.AddSingleton<IInfoProvider, ClientInfoProvider>();

        @this.AddSingleton<ClientCommand>()
             .AddSingleton<ICommand>(s => s.GetRequiredService<ClientCommand>());
        @this.AddSingleton<ICommand, ExitCommand>();
        @this.AddSingleton<ICommand, InfoCommand>();
        @this.AddSingleton<ICommand, KeyCommand>();
        @this.AddSingleton<NodeCommand>()
             .AddSingleton<ICommand>(s => s.GetRequiredService<NodeCommand>());
        @this.AddSingleton<ICommand, TxCommand>();
        return @this;
    }
}
