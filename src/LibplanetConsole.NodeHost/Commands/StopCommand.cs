using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Nodes;

namespace LibplanetConsole.NodeHost.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Stop node.")]
[method: ImportingConstructor]
internal sealed class StopCommand(INode node) : CommandAsyncBase
{
    public override bool IsEnabled => node.IsRunning is true;

    protected override async Task OnExecuteAsync(
        CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
    {
        await node.StopAsync(cancellationToken);
    }
}
