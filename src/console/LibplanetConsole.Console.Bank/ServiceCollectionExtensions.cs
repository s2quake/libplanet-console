using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Bank;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBank(this IServiceCollection @this)
    {
        @this.AddScoped<BankNode>()
             .AddScoped<IBank>(s => s.GetRequiredService<BankNode>())
             .AddScoped<INodeContent>(s => s.GetRequiredService<BankNode>());
        @this.AddScoped<BankClient>()
             .AddScoped<IBank>(s => s.GetRequiredService<BankClient>())
             .AddScoped<IClientContent>(s => s.GetRequiredService<BankClient>());

        return @this;
    }
}
