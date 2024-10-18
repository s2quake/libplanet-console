using LibplanetConsole.Node.Bank;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Node.Guild;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGuild(
        this IServiceCollection @this)
    {
        @this.AddSingleton<ICurrencyProvider, GuildGoldCurrencyProvider>();

        return @this;
    }
}
