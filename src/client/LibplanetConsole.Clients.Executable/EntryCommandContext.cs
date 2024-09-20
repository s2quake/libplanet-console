using JSSoft.Commands;

namespace LibplanetConsole.Clients.Executable;

[CommandSummary("Run a client or provide related tools to connect to the node.")]
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
