using JSSoft.Commands;

namespace LibplanetConsole.Console.Commands.Nodes;

[CommandSummary("Provides commands for node processes.")]
internal sealed class NodeProcessCommand(NodeCommand nodeCommand)
    : CommandMethodBase(nodeCommand, "process")
{
}
