using System.ComponentModel.Composition;
using JSSoft.Commands;
using JSSoft.Terminals;
using LibplanetConsole.Nodes.Executable.Commands;

namespace LibplanetConsole.Nodes.Executable;

[Export(typeof(CommandContext))]
[CommandSummary("Provides a prompt for input and execution of commands.")]
[CommandDescription("REPL for libplanet node.")]
[method: ImportingConstructor]
internal sealed class CommandContext(
    [ImportMany] IEnumerable<ICommand> commands,
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
