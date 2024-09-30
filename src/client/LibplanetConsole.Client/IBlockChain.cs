using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Crypto;
using LibplanetConsole.Common;

namespace LibplanetConsole.Client;

public interface IBlockChain
{
    event EventHandler<BlockEventArgs>? BlockAppended;

    Task<TxId> SendTransactionAsync(IAction[] actions, CancellationToken cancellationToken);

    Task<long> GetNextNonceAsync(Address address, CancellationToken cancellationToken);

    Task<AppHash> GetTipHashAsync(CancellationToken cancellationToken);

    Task<IValue> GetStateAsync(
        AppHash blockHash,
        Address accountAddress,
        Address address,
        CancellationToken cancellationToken);

    Task<IValue> GetStateByStateRootHashAsync(
        AppHash stateRootHash,
        Address accountAddress,
        Address address,
        CancellationToken cancellationToken);

    Task<AppHash> GetBlockHashAsync(long height, CancellationToken cancellationToken);

    Task<T> GetActionAsync<T>(TxId txId, int actionIndex, CancellationToken cancellationToken)
        where T : IAction;
}
