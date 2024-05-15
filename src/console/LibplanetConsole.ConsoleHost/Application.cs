using JSSoft.Commands.Extensions;
using JSSoft.Terminals;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Consoles;

namespace LibplanetConsole.ConsoleHost;

internal sealed partial class Application(ApplicationOptions options)
    : ApplicationBase(options), IApplication
{
    private SystemTerminal? _terminal;

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        _terminal = this.GetService<SystemTerminal>();
        await base.OnStartAsync(cancellationToken);
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
