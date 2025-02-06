using JSSoft.Commands;
using LibplanetConsole.Console.Guild.Commands;

namespace LibplanetConsole.Console.Guild;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGuild(this IServiceCollection @this)
    {
        @this.AddSingleton<Guild>()
             .AddSingleton<IGuild>(s => s.GetRequiredService<Guild>())
             .AddSingleton<IConsoleContent>(s => s.GetRequiredService<Guild>());
        @this.AddKeyedScoped<NodeGuild>(INode.Key)
            .AddKeyedScoped<INodeGuild>(
                INode.Key, (s, k) => s.GetRequiredKeyedService<NodeGuild>(k))
            .AddKeyedScoped<INodeContent>(
                INode.Key, (s, k) => s.GetRequiredKeyedService<NodeGuild>(k));
        @this.AddKeyedScoped<ClientGuild>(IClient.Key)
            .AddKeyedScoped<IClientGuild>(
                IClient.Key, (s, k) => s.GetRequiredKeyedService<ClientGuild>(k))
            .AddKeyedScoped<IClientContent>(
                IClient.Key, (s, k) => s.GetRequiredKeyedService<ClientGuild>(k));

        @this.AddSingleton<ICommand, GuildCommand>();
        @this.AddSingleton<ICommand, NodeGuildCommand>();
        @this.AddSingleton<ICommand, ClientGuildCommand>();
        @this.AddHostedService<AliasProvider>();

        return @this;
    }
}
