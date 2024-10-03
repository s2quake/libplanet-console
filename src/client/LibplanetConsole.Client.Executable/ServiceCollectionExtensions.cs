using JSSoft.Commands;
using LibplanetConsole.Client.Example;
using LibplanetConsole.Client.Executable.Commands;
using LibplanetConsole.Client.Executable.Tracers;
using LibplanetConsole.Framework;
using LibplanetConsole.Framework.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Client.Executable;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddClientExecutable(this IServiceCollection @this)
    {
        @this.AddSingleton<CommandContext>();
        @this.AddSingleton<SystemTerminal>();

        @this.AddSingletonWithInterface<ICommand, HelpCommand>();
        @this.AddSingletonWithInterface<ICommand, VersionCommand>();

        @this.AddSingleton<IApplicationService, BlockChainEventTracer>();
        @this.AddSingleton<IApplicationService, ClientEventTracer>();

        @this.AddExample();

        return @this;
    }
}
