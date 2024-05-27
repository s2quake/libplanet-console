using JSSoft.Commands.Extensions;
using JSSoft.Terminals;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Nodes.Executable;

internal sealed class Application(ApplicationOptions options)
    : ApplicationBase(options), IApplication
{
    private readonly ApplicationOptions _options = options;
    private SystemTerminal? _terminal;

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        if (_options.ParentProcessId == 0)
        {
            var sw = new StringWriter();
            var commandContext = this.GetService<CommandContext>();
            commandContext.Out = sw;
            _terminal = this.GetService<SystemTerminal>();
            await base.OnStartAsync(cancellationToken);
            sw.WriteSeparator(TerminalColorType.BrightGreen);
            await commandContext.ExecuteAsync(["--help"], cancellationToken: default);
            sw.WriteLine();
            await commandContext.ExecuteAsync(args: [], cancellationToken: default);
            sw.WriteSeparator(TerminalColorType.BrightGreen);
            commandContext.Out = Console.Out;
            sw.WriteLineIf(_options.AutoStart != true, GetStartupMessage());
            Console.Write(sw.ToString());

            await _terminal.StartAsync(cancellationToken);
        }
        else
        {
            await base.OnStartAsync(cancellationToken);
        }
    }

    private static string GetStartupMessage()
    {
        var startText = TerminalStringBuilder.GetString("start", TerminalColorType.Green);
        return $"\nType '{startText}' to start the node.";
    }
}
