using System.ComponentModel.Composition;
using JSSoft.Commands;

namespace LibplanetConsole.Node.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Stop node.")]
[method: ImportingConstructor]
internal sealed class StopCommand(INode node) : CommandAsyncBase
{
    public override bool IsEnabled => node.IsRunning is true;

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        await node.StopAsync(cancellationToken);
    }
}
