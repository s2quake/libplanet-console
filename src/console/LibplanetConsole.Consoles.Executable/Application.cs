using JSSoft.Commands.Extensions;
using JSSoft.Terminals;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Consoles.Executable;

internal sealed partial class Application(ApplicationOptions options)
    : ApplicationBase(options), IApplication
{
    private SystemTerminal? _terminal;

    protected override async Task OnRunAsync(CancellationToken cancellationToken)
    {
        var message = "Welcome to console for Libplanet.";
        Console.Out.WriteColoredLine(message, TerminalColorType.BrightGreen);
        _terminal = this.GetService<SystemTerminal>();
        await base.OnRunAsync(cancellationToken);
        await PrepareCommandContext();
        await _terminal.StartAsync(cancellationToken);

        async Task PrepareCommandContext()
        {
            var sw = new StringWriter();
            var commandContext = this.GetService<CommandContext>();
            commandContext.Out = sw;
            sw.WriteSeparator(TerminalColorType.BrightGreen);
            await commandContext.ExecuteAsync(["--help"], cancellationToken: default);
            sw.WriteLine();
            await commandContext.ExecuteAsync(args: [], cancellationToken: default);
            sw.WriteSeparator(TerminalColorType.BrightGreen);
            commandContext.Out = Console.Out;
            Console.Write(sw.ToString());
        }
    }
}
