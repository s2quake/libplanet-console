using System.ComponentModel.Composition;
using JSSoft.Commands;

namespace LibplanetConsole.Clients.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Stop client.")]
[method: ImportingConstructor]
internal sealed class StopCommand(IClient client) : CommandAsyncBase
{
    public override bool IsEnabled => client.IsRunning is true;

    protected override Task OnExecuteAsync(
        CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
        => client.StopAsync(cancellationToken);
}
