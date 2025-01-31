using JSSoft.Commands;
using LibplanetConsole.Bank;
using LibplanetConsole.Console.Bank.Commands;

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
        @this.AddKeyedScoped<ClientBank>(IClient.Key)
            .AddKeyedScoped<IClientBank>(
                IClient.Key, (s, k) => s.GetRequiredKeyedService<ClientBank>(k))
            .AddKeyedScoped<IClientContent>(
                IClient.Key, (s, k) => s.GetRequiredKeyedService<ClientBank>(k));
        @this.AddSingleton<CurrencyCollection>()
            .AddSingleton<ICurrencyCollection>(s => s.GetRequiredService<CurrencyCollection>());

        @this.AddSingleton<ICommand, BankCommand>();
        @this.AddSingleton<ICommand, NodeBankCommand>();
        @this.AddSingleton<ICommand, ClientBankCommand>();

        return @this;
    }
}
