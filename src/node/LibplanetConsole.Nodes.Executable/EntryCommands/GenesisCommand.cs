using JSSoft.Commands;

namespace LibplanetConsole.Nodes.Executable.EntryCommands;

internal sealed class GenesisCommand : CommandBase
{
    [CommandProperty("output", 'o')]
    public string OutputPath { get; set; } = string.Empty;

    protected override void OnExecute()
    {
        throw new NotImplementedException();
    }
}
