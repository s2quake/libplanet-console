using JSSoft.Commands;

namespace LibplanetConsole.Node.Executable;

[CommandSummary("Runs a node and provides related tools")]
internal sealed class EntryCommandContext(params ICommand[] commands)
    : CommandContextBase(commands)
{
    protected override void OnEmptyExecute()
    {
        if (GetCommand(["help"]) is IExecutable executable)
        {
            executable.Execute();
        }
    }
}
