using JSSoft.Commands;

namespace LibplanetConsole.Console.Commands.Nodes;

[CommandSummary("Provides commands for the node process")]
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
