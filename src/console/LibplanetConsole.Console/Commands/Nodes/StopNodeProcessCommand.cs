using JSSoft.Commands;
using LibplanetConsole.Console.Extensions;

namespace LibplanetConsole.Console.Commands.Nodes;

[CommandSummary("Stop node process")]
internal sealed class StopNodeProcessCommand(
    NodeProcessCommand processCommand,
    INodeCollection nodes)
    : CommandAsyncBase(processCommand, "stop")
{
    [CommandPropertyRequired(DefaultValue = "")]
    [CommandPropertyCompletion(nameof(GetNodeAddresses))]
    [CommandSummary("The address of the node. If not specified, the current node is used")]
    public string Address { get; set; } = string.Empty;

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var address = Address;
        var node = nodes.GetNodeOrCurrent(address);
        await node.StopProcessAsync(cancellationToken);
    }

    private string[] GetNodeAddresses()
        => nodes.Select(node => node.Address.ToString()).ToArray();
}
