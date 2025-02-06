using JSSoft.Commands;
using LibplanetConsole.Console.Delegation.Commands;

namespace LibplanetConsole.Console.Delegation;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDelegation(this IServiceCollection @this)
    {
        @this.AddSingleton<Delegation>()
            .AddSingleton<IDelegation>(s => s.GetRequiredService<Delegation>())
            .AddSingleton<IConsoleContent>(s => s.GetRequiredService<Delegation>());
        @this.AddKeyedScoped<NodeDelegation>(INode.Key)
            .AddKeyedScoped<INodeDelegation>(
                INode.Key, (s, k) => s.GetRequiredKeyedService<NodeDelegation>(k))
            .AddKeyedScoped<INodeContent>(
                INode.Key, (s, k) => s.GetRequiredKeyedService<NodeDelegation>(k));
        @this.AddKeyedScoped<ClientDelegation>(IClient.Key)
            .AddKeyedScoped<IClientDelegation>(
                IClient.Key, (s, k) => s.GetRequiredKeyedService<ClientDelegation>(k))
            .AddKeyedScoped<IClientContent>(
                IClient.Key, (s, k) => s.GetRequiredKeyedService<ClientDelegation>(k));

        @this.AddSingleton<ICommand, DelegationCommand>();
        @this.AddSingleton<ICommand, NodeDelegationCommand>();
        @this.AddSingleton<ICommand, ClientDelegationCommand>();

        return @this;
    }
}
