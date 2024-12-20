using JSSoft.Commands;

namespace LibplanetConsole.Console.Commands.Nodes;

[CommandSummary("Stops the node process")]
internal sealed class StopNodeProcessCommand(
    NodeProcessCommand processCommand,
    IServiceProvider serviceProvider)
    : NodeCommandAsyncBase(serviceProvider, processCommand, "stop")
{
    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var node = Node;
        await node.StopProcessAsync(cancellationToken);
    }
}
