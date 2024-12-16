using JSSoft.Commands;
using LibplanetConsole.Client.Delegation.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Client.Delegation;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDelegation(this IServiceCollection @this)
    {
        @this.AddSingleton<Delegation>()
            .AddSingleton<IDelegation>(s => s.GetRequiredService<Delegation>())
            .AddSingleton<IClientContent>(s => s.GetRequiredService<Delegation>());

        @this.AddSingleton<ICommand, DelegationCommand>();

        return @this;
    }
}
