using JSSoft.Commands;
using LibplanetConsole.Console.Delegation.Commands;
using Microsoft.Extensions.DependencyInjection;

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

        @this.AddSingleton<ICommand, DelegationCommand>();
        @this.AddSingleton<ICommand, NodeDelegationCommand>();

        return @this;
    }
}
