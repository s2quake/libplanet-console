using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Clients;

namespace LibplanetConsole.ClientHost.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Stop client.")]
[method: ImportingConstructor]
internal sealed class StopCommand(IClient client) : CommandAsyncBase
{
    public override bool IsEnabled => client.IsRunning is true;

    protected override async Task OnExecuteAsync(
        CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
    {
        await client.StopAsync(cancellationToken);
    }
}
