using JSSoft.Commands;
using LibplanetConsole.Console.Evidence;
using LibplanetConsole.Console.Executable.Commands;
using LibplanetConsole.Console.Executable.Tracers;
using LibplanetConsole.Logging;

namespace LibplanetConsole.Console.Executable;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExecutable(
        this IServiceCollection @this, ApplicationOptions options)
    {
        @this.AddLogging(options.LogPath, options.LibraryLogPath);

        @this.AddSingleton<CommandContext>();
        @this.AddSingleton<SystemTerminal>();
        @this.AddHostedService<ExecutableHostedService>();

        @this.AddSingleton<HelpCommand>()
             .AddSingleton<ICommand>(s => s.GetRequiredService<HelpCommand>());
        @this.AddSingleton<VersionCommand>()
             .AddSingleton<ICommand>(s => s.GetRequiredService<VersionCommand>());

        @this.AddHostedService<ClientCollectionEventTracer>();
        @this.AddHostedService<NodeCollectionEventTracer>();

        @this.AddEvidence();

        return @this;
    }
}
