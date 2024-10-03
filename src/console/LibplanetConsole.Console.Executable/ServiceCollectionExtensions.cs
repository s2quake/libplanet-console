using JSSoft.Commands;
using LibplanetConsole.Console.Executable.Commands;
using LibplanetConsole.Console.Executable.Tracers;
using LibplanetConsole.Framework;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Executable;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection @this, ApplicationOptions options)
    {
        @this.AddSingleton(s => new Application(s, options));
        @this.AddSingleton<IApplication>(s => s.GetRequiredService<Application>());
        @this.AddSingleton(
            ApplicationFramework.CreateLogger(
                typeof(Application), options.LogPath, string.Empty));

        @this.AddSingleton<CommandContext>();
        @this.AddSingleton<SystemTerminal>();

        @this.AddSingleton<HelpCommand>()
             .AddSingleton<ICommand>(s => s.GetRequiredService<HelpCommand>());
        @this.AddSingleton<VersionCommand>()
             .AddSingleton<ICommand>(s => s.GetRequiredService<VersionCommand>());

        @this.AddSingleton<IApplicationService, ClientCollectionEventTracer>();
        @this.AddSingleton<IApplicationService, NodeCollectionEventTracer>();

        return @this;
    }
}
