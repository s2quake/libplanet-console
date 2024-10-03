using JSSoft.Commands;
using LibplanetConsole.Common;

namespace LibplanetConsole.Client.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Stop client.")]
internal sealed class StopCommand(IClient client) : CommandAsyncBase
{
    public override bool IsEnabled => client.IsRunning is true;

    protected override Task OnExecuteAsync(CancellationToken cancellationToken)
        => client.StopAsync(cancellationToken);
}
