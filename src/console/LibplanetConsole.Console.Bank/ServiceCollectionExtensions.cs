using JSSoft.Commands;
using LibplanetConsole.Console.Bank.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Bank;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBank(this IServiceCollection @this)
    {
        @this.AddSingleton<Bank>()
             .AddSingleton<IBank>(s => s.GetRequiredService<Bank>())
             .AddSingleton<IConsoleContent>(s => s.GetRequiredService<Bank>());
        @this.AddKeyedScoped<NodeBank>(INode.Key)
             .AddKeyedScoped<INodeBank>(
                INode.Key, (s, k) => s.GetRequiredKeyedService<NodeBank>(k))
             .AddKeyedScoped<INodeContent>(
                INode.Key, (s, k) => s.GetRequiredKeyedService<NodeBank>(k));

        @this.AddSingleton<ICommand, BankCommand>();
        @this.AddSingleton<ICommand, NodeBankCommand>();

        return @this;
    }
}
