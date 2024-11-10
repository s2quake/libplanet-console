using JSSoft.Commands;
using LibplanetConsole.Console.Extensions;

namespace LibplanetConsole.Console.Commands.Nodes;

[CommandSummary("Start node process.")]
internal sealed class StartNodeProcessCommand(
    NodeProcessCommand processCommand,
    INodeCollection nodes)
    : CommandAsyncBase(processCommand, "start")
{
    [CommandPropertyRequired(DefaultValue = "")]
    [CommandSummary("The address of the node. If not specified, the current node is used.")]
    public string Address { get; set; } = string.Empty;

    [CommandPropertySwitch]
    [CommandSummary("The console does not attach to the target process after the node process " +
                    "is started.")]
    public bool Detach { get; set; }

    [CommandPropertySwitch]
    [CommandSummary("The node process is started in a new window.")]
    public bool NewWindow { get; set; }

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var address = Address;
        var node = nodes.GetNodeOrCurrent(address);
        var options = new ProcessOptions
        {
            Detach = Detach,
            NewWindow = NewWindow,
        };
        await node.StartProcessAsync(options, cancellationToken);
    }
}
