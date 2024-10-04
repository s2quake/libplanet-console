using JSSoft.Commands;
using LibplanetConsole.Console.Evidence;
using LibplanetConsole.Console.Example;
using LibplanetConsole.Console.Executable.Commands;
using LibplanetConsole.Console.Executable.Tracers;
using LibplanetConsole.Console.Explorer;
using LibplanetConsole.Framework;
using LibplanetConsole.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Executable;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection @this, ApplicationOptions options)
    {
        @this.AddLogging(options.LogPath, options.LibraryLogPath);

        @this.AddSingleton(s => new Application(s, options));
        @this.AddSingleton<IApplication>(s => s.GetRequiredService<Application>());

        @this.AddSingleton<CommandContext>();
        @this.AddSingleton<SystemTerminal>();

        @this.AddSingleton<HelpCommand>()
             .AddSingleton<ICommand>(s => s.GetRequiredService<HelpCommand>());
        @this.AddSingleton<VersionCommand>()
             .AddSingleton<ICommand>(s => s.GetRequiredService<VersionCommand>());

        @this.AddSingleton<IApplicationService, ClientCollectionEventTracer>();
        @this.AddSingleton<IApplicationService, NodeCollectionEventTracer>();

        @this.AddExample();
        @this.AddEvidence();
        @this.AddExplorer();

        return @this;
    }
}
