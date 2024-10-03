using JSSoft.Commands;
using LibplanetConsole.Framework;
using LibplanetConsole.Node.Evidence;
using LibplanetConsole.Node.Example;
using LibplanetConsole.Node.Executable.Commands;
using LibplanetConsole.Node.Executable.Tracers;
using LibplanetConsole.Node.Explorer;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Node.Executable;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection @this, ApplicationOptions options)
    {
        @this.AddSingleton(s => new Application(s, options));
        @this.AddSingleton<IApplication>(s => s.GetRequiredService<Application>());
        @this.AddSingleton(
            ApplicationFramework.CreateLogger(
                typeof(Application), options.LogPath, options.LibraryLogPath));

        @this.AddSingleton<CommandContext>();
        @this.AddSingleton<SystemTerminal>();

        @this.AddSingleton<HelpCommand>()
             .AddSingleton<ICommand>(s => s.GetRequiredService<HelpCommand>());
        @this.AddSingleton<VersionCommand>()
             .AddSingleton<ICommand>(s => s.GetRequiredService<VersionCommand>());

        @this.AddSingleton<IApplicationService, BlockChainEventTracer>();
        @this.AddSingleton<IApplicationService, NodeEventTracer>();

        @this.AddEvidence();
        @this.AddExample();
        @this.AddExplorer();

        return @this;
    }
}
