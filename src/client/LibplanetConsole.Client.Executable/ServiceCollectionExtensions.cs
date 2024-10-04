using JSSoft.Commands;
using LibplanetConsole.Client.Example;
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
        @this.AddSingleton(s => new Application(s, options));
        @this.AddSingleton<IApplication>(s => s.GetRequiredService<Application>());

        @this.AddSingleton<CommandContext>();
        @this.AddSingleton<SystemTerminal>();

        @this.AddSingleton<HelpCommand>()
             .AddSingleton<ICommand>(s => s.GetRequiredService<HelpCommand>());
        @this.AddSingleton<VersionCommand>()
             .AddSingleton<ICommand>(s => s.GetRequiredService<VersionCommand>());

        @this.AddSingleton<IApplicationService, BlockChainEventTracer>();
        @this.AddSingleton<IApplicationService, ClientEventTracer>();

        @this.AddExample();
        @this.AddLogging(options.LogPath, string.Empty);

        return @this;
    }
}
