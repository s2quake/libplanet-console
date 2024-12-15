using JSSoft.Commands;
using LibplanetConsole.Console.Guild.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Guild;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGuild(this IServiceCollection @this)
    {
        @this.AddSingleton<Guild>()
             .AddSingleton<IGuild>(s => s.GetRequiredService<Guild>())
             .AddSingleton<IConsoleContent>(s => s.GetRequiredService<Guild>());

        @this.AddSingleton<ICommand, GuildCommand>();

        return @this;
    }
}
