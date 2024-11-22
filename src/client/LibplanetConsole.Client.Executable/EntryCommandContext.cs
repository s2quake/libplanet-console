using JSSoft.Commands;

namespace LibplanetConsole.Client.Executable;

[CommandSummary("Runs a client and provides tools for connecting to the node")]
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
