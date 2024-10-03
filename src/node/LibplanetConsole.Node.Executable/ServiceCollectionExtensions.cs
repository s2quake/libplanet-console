using JSSoft.Commands;
using LibplanetConsole.Framework;
using LibplanetConsole.Framework.Extensions;
using LibplanetConsole.Node.Executable.Commands;
using LibplanetConsole.Node.Executable.Tracers;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Node.Executable;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNodeExecutable(this IServiceCollection @this)
    {
        @this.AddSingleton<CommandContext>();
        @this.AddSingleton<SystemTerminal>();

        @this.AddSingletonWithInterface<ICommand, HelpCommand>();
        @this.AddSingletonWithInterface<ICommand, VersionCommand>();

        @this.AddSingleton<IApplicationService, BlockChainEventTracer>();
        @this.AddSingleton<IApplicationService, NodeEventTracer>();

        return @this;
    }
}
