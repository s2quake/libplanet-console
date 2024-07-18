using LibplanetConsole.Common;
using LibplanetConsole.Nodes.Serializations;

namespace LibplanetConsole.Nodes.Services;

public interface INodeService
{
    Task<NodeInfo> StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);

    Task<NodeInfo> GetInfoAsync(CancellationToken cancellationToken);

    Task<AppId> SendTransactionAsync(byte[] transaction, CancellationToken cancellationToken);

    Task<long> GetNextNonceAsync(AppAddress address, CancellationToken cancellationToken);
}
