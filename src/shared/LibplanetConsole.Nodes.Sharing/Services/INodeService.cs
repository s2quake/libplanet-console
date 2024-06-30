using Libplanet.Types.Tx;
using LibplanetConsole.Common;
using LibplanetConsole.Nodes.Serializations;

namespace LibplanetConsole.Nodes.Services;

public interface INodeService
{
    Task<NodeInfo> StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);

    Task<NodeInfo> GetInfoAsync(CancellationToken cancellationToken);

    Task<TxId> SendTransactionAsync(byte[] transaction, CancellationToken cancellationToken);

    Task<long> GetNextNonceAsync(AppAddress address, CancellationToken cancellationToken);
}
