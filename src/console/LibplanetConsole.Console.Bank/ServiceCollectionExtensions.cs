using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Console.Bank.Commands;
using LibplanetConsole.Console.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Bank;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBank(this IServiceCollection @this)
    {
        @this.AddScoped<BankNode>()
             .AddScoped<IBank>(s => s.GetRequiredService<BankNode>())
             .AddScoped<INodeContent>(s => s.GetRequiredService<BankNode>());
            //  .AddScoped<INodeContentService>(s => s.GetRequiredService<BankNode>());
        @this.AddScoped<BankClient>()
             .AddScoped<IBank>(s => s.GetRequiredService<BankClient>())
             .AddScoped<IClientContent>(s => s.GetRequiredService<BankClient>());
            //  .AddScoped<IClientContentService>(s => s.GetRequiredService<BankClient>());

        // @this.AddSingleton<ICommand, BalanceCommand>();
        // @this.AddSingleton<ICommand, BurnCommand>();
        @this.AddSingleton<ICommand, ClientCommand>();
        // @this.AddSingleton<ICommand, MintCommand>();
        @this.AddSingleton<ICommand, NodeCommand>();
        @this.AddSingleton<ICommand, PoolCommand>();
        // @this.AddSingleton<ICommand, TransferCommand>();

        return @this;
    }
}
