using JSSoft.Commands;
using LibplanetConsole.Console.Executable.Commands;
using LibplanetConsole.Console.Executable.Tracers;
using LibplanetConsole.Framework;
using LibplanetConsole.Framework.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Executable;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConsoleExecutable(this IServiceCollection @this)
    {
        @this.AddSingleton<CommandContext>();
        @this.AddSingleton<SystemTerminal>();

        @this.AddSingletonWithInterface<ICommand, HelpCommand>();
        @this.AddSingletonWithInterface<ICommand, VersionCommand>();

        @this.AddSingleton<IApplicationService, ClientCollectionEventTracer>();
        @this.AddSingleton<IApplicationService, NodeCollectionEventTracer>();

        return @this;
    }
}
