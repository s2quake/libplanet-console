using JSSoft.Commands;
using LibplanetConsole.Client.Bank.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Client.Bank;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBank(this IServiceCollection @this)
    {
        @this.AddSingleton<Bank>()
            .AddSingleton<IBank>(s => s.GetRequiredService<Bank>());

        @this.AddSingleton<CurrencyCollection>()
            .AddSingleton<ICurrencyCollection>(s => s.GetRequiredService<CurrencyCollection>())
            .AddSingleton<IClientContent>(s => s.GetRequiredService<CurrencyCollection>());

        @this.AddSingleton<ICommand, BankCommand>();

        return @this;
    }
}
