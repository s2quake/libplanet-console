using JSSoft.Commands;

namespace LibplanetConsole.Node.Commands;

[CommandSummary("Starts the node")]
internal sealed class StartCommand(Node node) : CommandAsyncBase
{
    public override bool IsEnabled => node.IsRunning is false;

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        await node.StartAsync(cancellationToken);
    }
}
