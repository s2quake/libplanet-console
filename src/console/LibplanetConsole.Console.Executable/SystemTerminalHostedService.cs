using JSSoft.Commands.Extensions;
using JSSoft.Terminals;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Console.Executable;

internal sealed class SystemTerminalHostedService(
    CommandContext commandContext, SystemTerminal terminal) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var message = "Welcome to console for Libplanet.";
        var sw = new StringWriter();
        commandContext.Out = sw;
        await System.Console.Out.WriteColoredLineAsync(message, TerminalColorType.BrightGreen);
        await sw.WriteSeparatorAsync(TerminalColorType.BrightGreen);
        await commandContext.ExecuteAsync(["--help"], cancellationToken);
        await sw.WriteLineAsync();
        await commandContext.ExecuteAsync(args: [], cancellationToken);
        await sw.WriteSeparatorAsync(TerminalColorType.BrightGreen);
        commandContext.Out = System.Console.Out;
        await System.Console.Out.WriteAsync(sw.ToString());

        await terminal.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await terminal.StopAsync(cancellationToken);
    }
}
