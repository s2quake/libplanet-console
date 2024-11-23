using JSSoft.Commands;

namespace LibplanetConsole.Client.Commands;

[CommandSummary("Stops the client")]
internal sealed class StopCommand(IClient client) : CommandAsyncBase
{
    public override bool IsEnabled => client.IsRunning is true;

    protected override Task OnExecuteAsync(CancellationToken cancellationToken)
        => client.StopAsync(cancellationToken);
}
