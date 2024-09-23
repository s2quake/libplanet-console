using System.ComponentModel.Composition;
using JSSoft.Commands;

namespace LibplanetConsole.Node.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Start node.")]
[method: ImportingConstructor]
internal sealed class StartCommand(Node node) : CommandAsyncBase
{
    public override bool IsEnabled => node.IsRunning is false;

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        await node.StartAsync(cancellationToken);
    }
}
