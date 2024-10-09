using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Console.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConsole(
        this IServiceCollection @this, ApplicationOptions options)
    {
        @this.AddSingleton(options);
        @this.AddSingleton(s => new NodeCollection(s, options.Nodes))
             .AddSingleton<INodeCollection>(s => s.GetRequiredService<NodeCollection>())
             .AddHostedService(s => s.GetRequiredService<NodeCollection>());
        @this.AddSingleton(s => new ClientCollection(s, options.Clients))
             .AddSingleton<IClientCollection>(s => s.GetRequiredService<ClientCollection>())
             .AddHostedService(s => s.GetRequiredService<ClientCollection>());

        @this.AddScoped(NodeFactory.Create)
             .AddScoped<INode>(s => s.GetRequiredService<Node>())
             .AddScoped<IBlockChain>(s => s.GetRequiredService<Node>());
        @this.AddScoped(ClientFactory.Create)
             .AddScoped<IClient>(s => s.GetRequiredService<Client>());

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
