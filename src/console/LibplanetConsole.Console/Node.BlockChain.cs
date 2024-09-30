using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Crypto;
using Libplanet.Types.Blocks;
using Libplanet.Types.Tx;
using LibplanetConsole.Common;
using LibplanetConsole.Node;
using LibplanetConsole.Node.Services;

namespace LibplanetConsole.Console;

internal sealed partial class Node
{
    public event EventHandler<BlockEventArgs>? BlockAppended;

    public Task<long> GetNextNonceAsync(Address address, CancellationToken cancellationToken)
        => _blockChainService.Service.GetNextNonceAsync(address, cancellationToken);

    public async Task<TxId> SendTransactionAsync(
        IAction[] actions, CancellationToken cancellationToken)
    {
        var privateKey = _nodeOptions.PrivateKey;
        var address = privateKey.Address;
        var nonce = await _blockChainService.Service.GetNextNonceAsync(address, cancellationToken);
        var genesisHash = _nodeInfo.GenesisHash;
        var tx = Transaction.Create(
            nonce: nonce,
            privateKey: (PrivateKey)privateKey,
            genesisHash: (BlockHash)genesisHash,
            actions: [.. actions.Select(item => item.PlainValue)]);
        var txId = await _blockChainService.Service.SendTransactionAsync(
            transaction: tx.Serialize(),
            cancellationToken: cancellationToken);

        return txId;
    }

    public Task<TxId> SendTransactionAsync(
        Transaction transaction, CancellationToken cancellationToken)
    {
        return _blockChainService.Service.SendTransactionAsync(
            transaction.Serialize(), cancellationToken);
    }

    void IBlockChainCallback.OnBlockAppended(BlockInfo blockInfo)
    {
        _nodeInfo = _nodeInfo with { TipHash = blockInfo.Hash };
        BlockAppended?.Invoke(this, new BlockEventArgs(blockInfo));
    }

    public Task<AppHash> GetTipHashAsync(CancellationToken cancellationToken)
        => _blockChainService.Service.GetTipHashAsync(cancellationToken);

    public async Task<IValue> GetStateAsync(
        AppHash blockHash,
        Address accountAddress,
        Address address,
        CancellationToken cancellationToken)
    {
        var bytes = await _blockChainService.Service.GetStateAsync(
            blockHash,
            accountAddress,
            address,
            cancellationToken);
        return _codec.Decode(bytes);
    }

    public async Task<IValue> GetStateByStateRootHashAsync(
        AppHash stateRootHash,
        Address accountAddress,
        Address address,
        CancellationToken cancellationToken)
    {
        var bytes = await _blockChainService.Service.GetStateByStateRootHashAsync(
            stateRootHash,
            accountAddress,
            address,
            cancellationToken);
        return _codec.Decode(bytes);
    }

    public Task<AppHash> GetBlockHashAsync(long height, CancellationToken cancellationToken)
        => _blockChainService.Service.GetBlockHashAsync(height, cancellationToken);

    public async Task<T> GetActionAsync<T>(
        TxId txId,
        int actionIndex,
        CancellationToken cancellationToken)
        where T : IAction
    {
        var bytes = await _blockChainService.Service.GetActionAsync(
            txId, actionIndex, cancellationToken);
        var value = _codec.Decode(bytes);
        if (Activator.CreateInstance(typeof(T)) is T action)
        {
            action.LoadPlainValue(value);
            return action;
        }

        throw new InvalidOperationException("Action not found.");
    }
}
