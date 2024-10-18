using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Console.Delegation.Commands;
using LibplanetConsole.Console.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Delegation;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDelegation(this IServiceCollection @this)
    {
        @this.AddScoped<DelegationNode>()
             .AddScoped<IDelegation>(s => s.GetRequiredService<DelegationNode>())
             .AddScoped<INodeContent>(s => s.GetRequiredService<DelegationNode>());
            //  .AddScoped<INodeContentService>(s => s.GetRequiredService<DelegationNode>());
        @this.AddScoped<DelegationClient>()
             .AddScoped<IDelegation>(s => s.GetRequiredService<DelegationClient>())
             .AddScoped<IClientContent>(s => s.GetRequiredService<DelegationClient>());
            //  .AddScoped<IClientContentService>(s => s.GetRequiredService<DelegationClient>());

        // @this.AddSingleton<ICommand, DelegateCommand>();
        // @this.AddSingleton<ICommand, PromoteCommand>();
        // @this.AddSingleton<ICommand, UndelegateCommand>();
        @this.AddSingleton<ICommand, ValidatorCommand>();

        return @this;
    }
}
