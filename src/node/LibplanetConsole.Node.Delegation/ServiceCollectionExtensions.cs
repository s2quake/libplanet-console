using JSSoft.Commands;
using LibplanetConsole.Node.Delegation.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Node.Delegation;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDelegation(this IServiceCollection @this)
    {
        @this.AddSingleton<Validator>()
             .AddSingleton<INodeContent>(s => s.GetRequiredService<Validator>())
             .AddSingleton<IValidator>(s => s.GetRequiredService<Validator>());

        @this.AddSingleton<ICommand, ValidatorCommand>();
        return @this;
    }
}
