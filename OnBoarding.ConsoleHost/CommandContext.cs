using System.ComponentModel.Composition;
using JSSoft.Library.Commands;
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
}
