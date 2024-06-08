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
        if (_options.NoREPL != true)
        {
            var sw = new StringWriter();
            var commandContext = this.GetService<CommandContext>();
            var startupCondition = _options.ManualStart == true && _options.ParentProcessId == 0;
            commandContext.Out = sw;
            _terminal = this.GetService<SystemTerminal>();
            await base.OnStartAsync(cancellationToken);
            sw.WriteSeparator(TerminalColorType.BrightGreen);
            await commandContext.ExecuteAsync(["--help"], cancellationToken: default);
            sw.WriteLine();
            await commandContext.ExecuteAsync(args: [], cancellationToken: default);
            sw.WriteSeparator(TerminalColorType.BrightGreen);
            commandContext.Out = Console.Out;
            sw.WriteLineIf(startupCondition, GetStartupMessage());
            Console.Write(sw.ToString());

            await _terminal.StartAsync(cancellationToken);
        }
        else if (_options.ParentProcessId == 0)
        {
            await base.OnStartAsync(cancellationToken);
            WaitInput();
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

    private async void WaitInput()
    {
        Console.WriteLine("Press any key to exit.");
        await Task.Run(() => Console.ReadKey(intercept: true));
        Cancel();
    }
}
