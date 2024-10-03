using JSSoft.Commands.Extensions;
using JSSoft.Terminals;
using LibplanetConsole.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Executable;

internal sealed partial class Application(
    IServiceProvider serviceProvider, ApplicationOptions options)
    : ApplicationBase(serviceProvider, options), IApplication
{
    protected override async Task OnRunAsync(CancellationToken cancellationToken)
    {
        var message = "Welcome to console for Libplanet.";
        var sw = new StringWriter();
        var commandContext = this.GetRequiredService<CommandContext>();
        var terminal = this.GetRequiredService<SystemTerminal>();
        commandContext.Out = sw;
        await System.Console.Out.WriteColoredLineAsync(message, TerminalColorType.BrightGreen);
        await base.OnRunAsync(cancellationToken);
        await sw.WriteSeparatorAsync(TerminalColorType.BrightGreen);
        await commandContext.ExecuteAsync(["--help"], cancellationToken: default);
        await sw.WriteLineAsync();
        await commandContext.ExecuteAsync(args: [], cancellationToken: default);
        await sw.WriteSeparatorAsync(TerminalColorType.BrightGreen);
        commandContext.Out = System.Console.Out;
        await System.Console.Out.WriteAsync(sw.ToString());
        await terminal.StartAsync(cancellationToken);
    }
}
