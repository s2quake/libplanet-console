using System.ComponentModel.Composition;
using System.Text;
using JSSoft.Library.Commands;
using JSSoft.Library.Terminals;
using OnBoarding.ConsoleHost.Commands;

namespace OnBoarding.ConsoleHost;

[Export(typeof(CommandContext))]
[CommandSummary("Provides a prompt for input and execution of commands.")]
[CommandDescription("REPL for OnBoarding")]
sealed class CommandContext : CommandContextBase
{
    protected override ICommand HelpCommand { get; }

    protected override ICommand VersionCommand { get; }

    [ImportingConstructor]
    public CommandContext([ImportMany] IEnumerable<ICommand> commands, HelpCommand helpCommand, VersionCommand versionCommand) : base(commands)
    {
        HelpCommand = helpCommand;
        VersionCommand = versionCommand;
    }

    protected override void OnEmptyExecute()
    {
        var sb = new StringBuilder();
        sb.AppendLine(TerminalStringBuilder.GetString("Type '--help | -h' for usage.", TerminalColorType.BrightGreen));
        sb.AppendLine(TerminalStringBuilder.GetString("Type 'exit' to exit application.", TerminalColorType.BrightGreen));
        Out.Write(sb.ToString());
    }
}
