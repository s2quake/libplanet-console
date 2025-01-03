using JSSoft.Commands;
using LibplanetConsole.Common;

namespace LibplanetConsole.Client.Commands;

[CommandSummary("Starts the client")]
internal sealed class StartCommand(Client client) : CommandAsyncBase
{
    public override bool IsEnabled => client.IsRunning is false;

    [CommandPropertyRequired(DefaultValue = "")]
    [CommandSummary("Specifies the end-point of the node to connect to")]
    public string NodeEndPoint { get; set; } = string.Empty;

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var nodeEndPoint = EndPointUtility.ParseOrFallback(NodeEndPoint, client.NodeEndPoint);
        client.NodeEndPoint = nodeEndPoint;
        await client.StartAsync(cancellationToken);
    }
}
