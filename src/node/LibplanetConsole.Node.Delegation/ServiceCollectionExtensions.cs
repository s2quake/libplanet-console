using JSSoft.Commands;
using LibplanetConsole.Node.Delegation.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Node.Delegation;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDelegation(this IServiceCollection @this)
    {
        @this.AddSingleton<Delegation>()
             .AddSingleton<IDelegation>(s => s.GetRequiredService<Delegation>());
            //  .AddSingleton<IApplicationService>(s => s.GetRequiredService<Delegation>());
        // @this.AddSingleton<IInfoProvider, DelegationInfoProvider>();
        // @this.AddSingleton<ILocalService, DelegationService>();
        @this.AddSingleton<ICommand, ClaimCommand>();
        @this.AddSingleton<ICommand, DelegateCommand>();
        @this.AddSingleton<ICommand, RewardPoolCommand>();
        @this.AddSingleton<ICommand, UndelegateCommand>();
        @this.AddSingleton<ICommand, ValidatorCommand>();
        return @this;
    }
}
