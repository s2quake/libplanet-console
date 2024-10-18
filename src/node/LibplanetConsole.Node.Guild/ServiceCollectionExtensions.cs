using JSSoft.Commands;
using LibplanetConsole.Node.Bank;
using LibplanetConsole.Node.Guild.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Node.Guild;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGuild(
        this IServiceCollection @this)
    {
        @this.AddSingleton<Guild>()
             .AddSingleton<INodeContent>(s => s.GetRequiredService<Guild>())
             .AddSingleton<IGuild>(s => s.GetRequiredService<Guild>());
        @this.AddSingleton<ICurrencyProvider, GuildGoldCurrencyProvider>();

        @this.AddSingleton<ICommand, GuildCommand>();

        return @this;
    }
}
