using Libplanet.Crypto;
using Libplanet.Types.Tx;
using LibplanetConsole.Nodes.Serializations;

namespace LibplanetConsole.Nodes.Services;

public interface INodeService
{
    Task<NodeInfo> StartAsync(NodeOptionsInfo nodeOptionsInfo, CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);

    Task<NodeInfo> GetInfoAsync(CancellationToken cancellationToken);

    Task<TxId> SendTransactionAsync(byte[] transaction, CancellationToken cancellationToken);

    Task<long> GetNextNonceAsync(Address address, CancellationToken cancellationToken);
}
