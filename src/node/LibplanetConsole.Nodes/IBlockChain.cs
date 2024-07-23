using Libplanet.Action;
using Libplanet.Types.Tx;
using LibplanetConsole.Common;

namespace LibplanetConsole.Nodes;

public interface IBlockChain
{
    event EventHandler<BlockEventArgs>? BlockAppended;

    Task<AppId> AddTransactionAsync(IAction[] actions, CancellationToken cancellationToken);

    Task AddTransactionAsync(Transaction transaction, CancellationToken cancellationToken);

    Task<long> GetNextNonceAsync(AppAddress address, CancellationToken cancellationToken);

    Task<AppHash> GetTipHashAsync(CancellationToken cancellationToken);

    Task<byte[]> GetStateByBlockHashAsync(
        AppHash blockHash,
        AppAddress accountAddress,
        AppAddress address,
        CancellationToken cancellationToken);

    Task<byte[]> GetStateByStateRootHashAsync(
        AppHash stateRootHash,
        AppAddress accountAddress,
        AppAddress address,
        CancellationToken cancellationToken);

    Task<AppHash> GetBlockHashAsync(long height, CancellationToken cancellationToken);

    Task<byte[]> GetActionAsync(AppId txId, int actionIndex, CancellationToken cancellationToken);
}
