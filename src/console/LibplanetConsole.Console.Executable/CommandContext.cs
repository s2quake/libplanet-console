using JSSoft.Commands;
using JSSoft.Terminals;
using LibplanetConsole.Common;
using LibplanetConsole.Console.Executable.Commands;

namespace LibplanetConsole.Console.Executable;

[CommandSummary("Provides a prompt for input and execution of commands.")]
[CommandDescription("REPL for libplanet.")]
internal sealed class CommandContext(
    IEnumerable<ICommand> commands,
    HelpCommand helpCommand,
    VersionCommand versionCommand)
    : CommandContextBase(commands)
{
    protected override ICommand HelpCommand { get; } = helpCommand;

    protected override ICommand VersionCommand { get; } = versionCommand;

    protected override void OnEmptyExecute()
    {
        var tsb = new TerminalStringBuilder
        {
            Foreground = TerminalColorType.BrightGreen,
        };
        tsb.AppendLine("Type '--help | -h' for usage.");
        tsb.AppendLine("Type 'exit' to exit application.");
        tsb.ResetOptions();
        tsb.Append(string.Empty);
        Out.Write(tsb.ToString());
    }
}
