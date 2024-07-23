using System.ComponentModel.Composition;
using Libplanet.Types.Tx;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Nodes.Services;

[Export(typeof(ILocalService))]
[method: ImportingConstructor]
internal sealed class BlockChainService(Node node)
    : LocalService<IBlockChainService>, IBlockChainService
{
    private readonly Node _node = node;

    public async Task<AppId> SendTransactionAsync(
        byte[] transaction, CancellationToken cancellationToken)
    {
        var tx = Transaction.Deserialize(transaction);
        await _node.AddTransactionAsync(tx, cancellationToken);
        return (AppId)tx.Id;
    }

    public Task<long> GetNextNonceAsync(AppAddress address, CancellationToken cancellationToken)
        => _node.GetNextNonceAsync(address, cancellationToken);

    public Task<AppHash> GetTipHashAsync(CancellationToken cancellationToken)
        => _node.GetTipHashAsync(cancellationToken);

    public Task<byte[]> GetStateAsync(
        AppHash blockHash,
        AppAddress accountAddress,
        AppAddress address,
        CancellationToken cancellationToken)
        => _node.GetStateAsync(blockHash, accountAddress, address, cancellationToken);

    public Task<byte[]> GetStateByStateRootHashAsync(
        AppHash stateRootHash,
        AppAddress accountAddress,
        AppAddress address,
        CancellationToken cancellationToken)
        => _node.GetStateByStateRootHashAsync(
            stateRootHash, accountAddress, address, cancellationToken);

    public Task<AppHash> GetBlockHashAsync(long height, CancellationToken cancellationToken)
        => _node.GetBlockHashAsync(height, cancellationToken);

    public Task<byte[]> GetActionAsync(
        AppId txId, int actionIndex, CancellationToken cancellationToken)
        => _node.GetActionAsync(txId, actionIndex, cancellationToken);
}
