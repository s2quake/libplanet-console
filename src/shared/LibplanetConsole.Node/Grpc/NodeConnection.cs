#if LIBPLANET_CONSOLE || LIBPLANET_CLIENT
using Grpc.Core;
using LibplanetConsole.Grpc;

namespace LibplanetConsole.Node.Grpc;

internal sealed class NodeConnection(NodeService nodeService)
    : ConnectionMonitor<NodeService>(nodeService, FuncAsync)
{
    private static async Task FuncAsync(
        NodeService nodeService, CancellationToken cancellationToken)
    {
        var callOptions = new CallOptions(
            deadline: DateTime.UtcNow.AddSeconds(10),
            cancellationToken: cancellationToken);
        await nodeService.PingAsync(new(), callOptions);
    }
}
#endif // LIBPLANET_CONSOLE || LIBPLANET_CLIENT
