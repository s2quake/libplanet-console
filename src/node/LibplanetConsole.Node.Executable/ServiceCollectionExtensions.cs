using JSSoft.Commands;
using LibplanetConsole.Framework;
using LibplanetConsole.Framework.Extensions;
using LibplanetConsole.Node.Evidence;
using LibplanetConsole.Node.Example;
using LibplanetConsole.Node.Executable.Commands;
using LibplanetConsole.Node.Executable.Tracers;
using LibplanetConsole.Node.Explorer;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Node.Executable;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExecutable(this IServiceCollection @this)
    {
        @this.AddSingleton<CommandContext>();
        @this.AddSingleton<SystemTerminal>();

        @this.AddSingletonWithInterface<ICommand, HelpCommand>();
        @this.AddSingletonWithInterface<ICommand, VersionCommand>();

        @this.AddSingleton<IApplicationService, BlockChainEventTracer>();
        @this.AddSingleton<IApplicationService, NodeEventTracer>();

        @this.AddEvidence();
        @this.AddExample();
        @this.AddExplorer();

        return @this;
    }
}
