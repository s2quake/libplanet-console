using JSSoft.Commands;

namespace LibplanetConsole.Console.Commands.Nodes;

[CommandSummary("Starts the node process")]
internal sealed class StartNodeProcessCommand(
    NodeProcessCommand processCommand,
    IServiceProvider serviceProvider)
    : NodeCommandAsyncBase(serviceProvider, processCommand, "start")
{
    [CommandPropertySwitch]
    [CommandSummary("If set, the console does not attach to the target process after starting " +
                    "the node process")]
    public bool Detach { get; set; }

    [CommandPropertySwitch]
    [CommandSummary("If set, the node process starts in a new window")]
    public bool NewWindow { get; set; }

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var node = CurrentNode;
        var options = new ProcessOptions
        {
            Detach = Detach,
            NewWindow = NewWindow,
        };
        await node.StartProcessAsync(options, cancellationToken);
    }
}
