using JSSoft.Commands;
using LibplanetConsole.Node.Bank;
using LibplanetConsole.Node.Hand.Commands;

namespace LibplanetConsole.Node.Hand;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHand(this IServiceCollection @this)
    {
        @this.AddSingleton<Hand>()
            .AddSingleton<INodeContent>(s => s.GetRequiredService<Hand>())
            .AddSingleton<IHand>(s => s.GetRequiredService<Hand>());
        @this.AddSingleton<ICurrencyProvider, CurrencyProvider>();

        @this.AddSingleton<ICommand, HandCommand>();
        return @this;
    }
}
