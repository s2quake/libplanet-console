using System.Security.Cryptography;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Crypto;
using LibplanetConsole.Common;

namespace LibplanetConsole.Node;

public interface IBlockChain
{
    event EventHandler<BlockEventArgs>? BlockAppended;

    Task<TxId> AddTransactionAsync(IAction[] actions, CancellationToken cancellationToken);

    Task<long> GetNextNonceAsync(Address address, CancellationToken cancellationToken);

    Task<BlockHash> GetTipHashAsync(CancellationToken cancellationToken);

    Task<IValue> GetStateAsync(
        BlockHash? blockHash,
        Address accountAddress,
        Address address,
        CancellationToken cancellationToken);

    Task<IValue> GetStateByStateRootHashAsync(
        HashDigest<SHA256> stateRootHash,
        Address accountAddress,
        Address address,
        CancellationToken cancellationToken);

    Task<BlockHash> GetBlockHashAsync(long height, CancellationToken cancellationToken);

    Task<T> GetActionAsync<T>(TxId txId, int actionIndex, CancellationToken cancellationToken)
        where T : IAction;
}
