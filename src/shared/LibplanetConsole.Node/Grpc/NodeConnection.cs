#if LIBPLANET_CONSOLE || LIBPLANET_CLIENT
using LibplanetConsole.Grpc;

namespace LibplanetConsole.Node.Grpc;

internal sealed class NodeConnection(NodeService nodeService)
    : ConnectionMonitor<NodeService>(nodeService, FuncAsync)
{
    private static async Task FuncAsync(
        NodeService nodeService, CancellationToken cancellationToken)
    {
        await nodeService.PingAsync(new(), cancellationToken: cancellationToken);
    }
}
#endif // LIBPLANET_CONSOLE || LIBPLANET_CLIENT
