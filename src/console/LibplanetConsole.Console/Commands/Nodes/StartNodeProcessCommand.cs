using JSSoft.Commands;
using LibplanetConsole.Console.Extensions;

namespace LibplanetConsole.Console.Commands.Nodes;

[CommandSummary("Starts the node process")]
internal sealed class StartNodeProcessCommand(
    NodeProcessCommand processCommand,
    INodeCollection nodes)
    : CommandAsyncBase(processCommand, "start")
{
    [CommandPropertyRequired(DefaultValue = "")]
    [CommandPropertyCompletion(nameof(GetNodeAddresses))]
    [CommandSummary("Speifies the address of the node")]
    public string Address { get; set; } = string.Empty;

    [CommandPropertySwitch]
    [CommandSummary("If set, the console does not attach to the target process after starting " +
                    "the node process")]
    public bool Detach { get; set; }

    [CommandPropertySwitch]
    [CommandSummary("If set, the node process starts in a new window")]
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

    private string[] GetNodeAddresses()
        => nodes.Select(node => node.Address.ToString()).ToArray();
}
