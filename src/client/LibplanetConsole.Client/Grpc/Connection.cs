namespace LibplanetConsole.Client.Grpc;

internal sealed class Connection(NodeService nodeService)
    : ConnectionMonitor<NodeService>(nodeService, FuncAsync)
{
    private static async Task FuncAsync(NodeService client, CancellationToken cancellationToken)
    {
        await client.PingAsync(new(), cancellationToken: cancellationToken);
    }
}
