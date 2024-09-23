using Bencodex.Types;
using Libplanet.Action;
using LibplanetConsole.Common;

namespace LibplanetConsole.Clients;

public interface IBlockChain
{
    event EventHandler<BlockEventArgs>? BlockAppended;

    Task<AppId> SendTransactionAsync(IAction[] actions, CancellationToken cancellationToken);

    Task<long> GetNextNonceAsync(AppAddress address, CancellationToken cancellationToken);

    Task<AppHash> GetTipHashAsync(CancellationToken cancellationToken);

    Task<IValue> GetStateAsync(
        AppHash blockHash,
        AppAddress accountAddress,
        AppAddress address,
        CancellationToken cancellationToken);

    Task<IValue> GetStateByStateRootHashAsync(
        AppHash stateRootHash,
        AppAddress accountAddress,
        AppAddress address,
        CancellationToken cancellationToken);

    Task<AppHash> GetBlockHashAsync(long height, CancellationToken cancellationToken);

    Task<T> GetActionAsync<T>(AppId txId, int actionIndex, CancellationToken cancellationToken)
        where T : IAction;
}
