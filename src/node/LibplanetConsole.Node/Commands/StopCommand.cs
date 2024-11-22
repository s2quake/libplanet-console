using JSSoft.Commands;

namespace LibplanetConsole.Node.Commands;

[CommandSummary("Stops the node")]
internal sealed class StopCommand(INode node) : CommandAsyncBase
{
    public override bool IsEnabled => node.IsRunning is true;

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        await node.StopAsync(cancellationToken);
    }
}
