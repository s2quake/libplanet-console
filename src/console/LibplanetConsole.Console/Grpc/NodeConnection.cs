namespace LibplanetConsole.Console.Grpc;

internal sealed class NodeConnection(NodeService nodeService)
    : ConnectionMonitor<NodeService>(nodeService, FuncAsync)
{
    private static async Task FuncAsync(
        NodeService nodeService, CancellationToken cancellationToken)
    {
        await nodeService.PingAsync(new(), cancellationToken: cancellationToken);
    }
}
