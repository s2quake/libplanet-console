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
            .AddSingleton<IGuild>(s => s.GetRequiredService<Guild>())
            .AddSingleton<INodeContent>(s => s.GetRequiredService<Guild>())
            .AddSingleton<ICurrencyProvider>(s => s.GetRequiredService<Guild>());
        @this.AddSingleton<ICurrencyProvider, CurrencyProvider>();

        @this.AddSingleton<ICommand, GuildCommand>();

        return @this;
    }
}
