using JSSoft.Commands;
using LibplanetConsole.Node.Bank.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Node.Bank;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBank(this IServiceCollection @this)
    {
        @this.AddSingleton<Bank>()
             .AddSingleton<INodeContent>(s => s.GetRequiredService<Bank>())
             .AddSingleton<IBank>(s => s.GetRequiredService<Bank>());
        @this.AddSingleton<ICommand, BankCommand>();
        // @this.AddSingleton<ICommand, BurnCommand>();
        // @this.AddSingleton<ICommand, BurnCommand>();
        // @this.AddSingleton<ICommand, MintCommand>();
        // @this.AddSingleton<ICommand, PoolCommand>();
        // @this.AddSingleton<ICommand, TransferCommand>();
        return @this;
    }
}
