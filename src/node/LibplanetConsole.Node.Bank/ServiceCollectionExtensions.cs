using JSSoft.Commands;
using LibplanetConsole.Node.Bank.Commands;

namespace LibplanetConsole.Node.Bank;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBank(this IServiceCollection @this)
    {
        @this.AddSingleton<Bank>()
            .AddSingleton<INodeContent>(s => s.GetRequiredService<Bank>())
            .AddSingleton<IBank>(s => s.GetRequiredService<Bank>());

        @this.AddSingleton<CurrencyCollection>()
            .AddSingleton<ICurrencyCollection>(s => s.GetRequiredService<CurrencyCollection>())
            .AddSingleton<INodeContent>(s => s.GetRequiredService<CurrencyCollection>());

        @this.AddSingleton<ICommand, BankCommand>();
        return @this;
    }
}
