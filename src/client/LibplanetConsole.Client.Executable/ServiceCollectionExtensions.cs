using JSSoft.Commands;
using LibplanetConsole.Client.Executable.Commands;
using LibplanetConsole.Client.Executable.Tracers;
using LibplanetConsole.Framework;
using LibplanetConsole.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Client.Executable;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection @this, ApplicationOptions options)
    {
        @this.AddLogging(options.LogPath, string.Empty);

        @this.AddSingleton<CommandContext>();
        @this.AddSingleton<SystemTerminal>();
        @this.AddHostedService<TerminalHostedService>();

        @this.AddSingleton<HelpCommand>()
             .AddSingleton<ICommand>(s => s.GetRequiredService<HelpCommand>());
        @this.AddSingleton<VersionCommand>()
             .AddSingleton<ICommand>(s => s.GetRequiredService<VersionCommand>());

        @this.AddHostedService<BlockChainEventTracer>();
        @this.AddHostedService<ClientEventTracer>();

        return @this;
    }
}
