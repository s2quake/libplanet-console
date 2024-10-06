using JSSoft.Commands;
using LibplanetConsole.Framework;
using LibplanetConsole.Logging;
using LibplanetConsole.Node.Evidence;
using LibplanetConsole.Node.Executable.Commands;
using LibplanetConsole.Node.Executable.Tracers;
using LibplanetConsole.Node.Explorer;

namespace LibplanetConsole.Node.Executable;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection @this, ApplicationOptions options)
    {
        @this.AddLogging(options.LogPath, options.LibraryLogPath);

        @this.AddSingleton<CommandContext>();
        @this.AddSingleton<SystemTerminal>();
        @this.AddHostedService<TerminalHostedService>();

        @this.AddSingleton<HelpCommand>()
             .AddSingleton<ICommand>(s => s.GetRequiredService<HelpCommand>());
        @this.AddSingleton<VersionCommand>()
             .AddSingleton<ICommand>(s => s.GetRequiredService<VersionCommand>());

        @this.AddHostedService<BlockChainEventTracer>();
        @this.AddHostedService<NodeEventTracer>();

        @this.AddEvidence();

        @this.AddGrpc();

        return @this;
    }
}