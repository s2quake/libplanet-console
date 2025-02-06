using JSSoft.Commands;
using LibplanetConsole.Client.Guild.Commands;

namespace LibplanetConsole.Client.Guild;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGuild(this IServiceCollection @this)
    {
        @this.AddSingleton<Guild>()
            .AddSingleton<IGuild>(s => s.GetRequiredService<Guild>());

        @this.AddSingleton<ICommand, GuildCommand>();

        return @this;
    }
}
