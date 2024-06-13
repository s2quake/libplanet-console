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
        await Console.Out.WriteColoredLineAsync(message, TerminalColorType.BrightGreen);
        _terminal = this.GetService<SystemTerminal>();
        await base.OnRunAsync(cancellationToken);
        await PrepareCommandContext();
        await _terminal.StartAsync(cancellationToken);

        async Task PrepareCommandContext()
        {
            var sw = new StringWriter();
            var commandContext = this.GetService<CommandContext>();
            commandContext.Out = sw;
            await sw.WriteSeparatorAsync(TerminalColorType.BrightGreen);
            await commandContext.ExecuteAsync(["--help"], cancellationToken: default);
            await sw.WriteLineAsync();
            await commandContext.ExecuteAsync(args: [], cancellationToken: default);
            await sw.WriteSeparatorAsync(TerminalColorType.BrightGreen);
            commandContext.Out = Console.Out;
            await Console.Out.WriteAsync(sw.ToString());
        }
    }
}
