using JSSoft.Commands;

namespace LibplanetConsole.Consoles.Executable.EntryCommands;

[CommandSummary("Run the Libplanet console.")]
internal sealed class InitializeCommand : CommandBase
{
    [CommandPropertyRequired]
    [CommandSummary("The directory path to initialize.")]
    public string OutputPath { get; set; } = string.Empty;

    protected override void OnExecute()
    {
        throw new NotImplementedException();
    }
}
