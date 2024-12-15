using JSSoft.Commands;
using LibplanetConsole.Node.Delegation.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Node.Delegation;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDelegation(this IServiceCollection @this)
    {
        @this.AddSingleton<Delegation>()
             .AddSingleton<INodeContent>(s => s.GetRequiredService<Delegation>())
             .AddSingleton<IDelegation>(s => s.GetRequiredService<Delegation>());

        @this.AddSingleton<ICommand, DelegationCommand>();
        return @this;
    }
}
