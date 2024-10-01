using System.Security.Cryptography;

namespace LibplanetConsole.Node;

public interface IBlockChain
{
    event EventHandler<BlockEventArgs>? BlockAppended;

    Task<TxId> SendTransactionAsync(IAction[] actions, CancellationToken cancellationToken);

    Task<long> GetNextNonceAsync(Address address, CancellationToken cancellationToken);

    Task<BlockHash> GetTipHashAsync(CancellationToken cancellationToken);

    Task<IValue> GetStateAsync(
        Address accountAddress, Address address, CancellationToken cancellationToken)
        => GetStateAsync(blockHash: null, accountAddress, address, cancellationToken);

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

    Task<FungibleAssetValue> GetBalanceAsync(
        Address address, Currency currency, CancellationToken cancellationToken);
}
