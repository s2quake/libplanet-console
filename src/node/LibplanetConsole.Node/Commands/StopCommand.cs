using JSSoft.Commands;
using LibplanetConsole.Common;

namespace LibplanetConsole.Node.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Stop node.")]
internal sealed class StopCommand(INode node) : CommandAsyncBase
{
    public override bool IsEnabled => node.IsRunning is true;

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        await node.StopAsync(cancellationToken);
    }
}
