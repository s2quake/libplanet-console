using JSSoft.Commands;
using JSSoft.Commands.Extensions;

namespace LibplanetConsole.Console.Commands.Nodes;

[CommandSummary("Provides commands for node processes.")]
internal sealed class NodeProcessCommand(NodeCommand nodeCommand)
    : CommandMethodBase(nodeCommand, "process"), IExecutable
{
    public void Execute()
    {
        if (Context.HelpCommand is HelpCommandBase helpCommand)
        {
            helpCommand.PrintHelp(this);
        }
    }
}
