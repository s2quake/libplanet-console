using JSSoft.Commands;

namespace LibplanetConsole.Console.Executable;

[CommandSummary("Run nodes and clients and provide related tools")]
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
