using System.Security.Cryptography;
using LibplanetConsole.Node;

namespace LibplanetConsole.Console;

internal sealed partial class Node
{
    public event EventHandler<BlockEventArgs>? BlockAppended;

    public Task<long> GetNextNonceAsync(Address address, CancellationToken cancellationToken)
    {
        // return _blockChainService.Service.GetNextNonceAsync(address, cancellationToken);
        throw new NotImplementedException();
    }

    public async Task<TxId> SendTransactionAsync(
        IAction[] actions, CancellationToken cancellationToken)
    {
        // var privateKey = _nodeOptions.PrivateKey;
        // var address = privateKey.Address;
        // var nonce = await _blockChainService.Service.GetNextNonceAsync(address, cancellationToken);
        // var genesisHash = _nodeInfo.GenesisHash;
        // var tx = Transaction.Create(
        //     nonce: nonce,
        //     privateKey: privateKey,
        //     genesisHash: genesisHash,
        //     actions: [.. actions.Select(item => item.PlainValue)]);
        // var txId = await _blockChainService.Service.SendTransactionAsync(
        //     transaction: tx.Serialize(),
        //     cancellationToken: cancellationToken);

        // return txId;
        throw new NotImplementedException();
    }

    public Task<TxId> SendTransactionAsync(
        Transaction transaction, CancellationToken cancellationToken)
    {
        // return _blockChainService.Service.SendTransactionAsync(
        //     transaction.Serialize(), cancellationToken);
        throw new NotImplementedException();
    }

    // void IBlockChainCallback.OnBlockAppended(BlockInfo blockInfo)
    // {
    //     _nodeInfo = _nodeInfo with { TipHash = blockInfo.Hash };
    //     BlockAppended?.Invoke(this, new BlockEventArgs(blockInfo));
    // }

    public Task<BlockHash> GetTipHashAsync(CancellationToken cancellationToken)
    {
        // return _blockChainService.Service.GetTipHashAsync(cancellationToken);
        throw new NotImplementedException();
    }

    public async Task<IValue> GetStateAsync(
        BlockHash? blockHash,
        Address accountAddress,
        Address address,
        CancellationToken cancellationToken)
    {
        // var bytes = await _blockChainService.Service.GetStateAsync(
        //     blockHash,
        //     accountAddress,
        //     address,
        //     cancellationToken);
        // return _codec.Decode(bytes);
        throw new NotImplementedException();
    }

    public async Task<IValue> GetStateByStateRootHashAsync(
        HashDigest<SHA256> stateRootHash,
        Address accountAddress,
        Address address,
        CancellationToken cancellationToken)
    {
        // var bytes = await _blockChainService.Service.GetStateByStateRootHashAsync(
        //     stateRootHash,
        //     accountAddress,
        //     address,
        //     cancellationToken);
        // return _codec.Decode(bytes);
        throw new NotImplementedException();
    }

    public Task<BlockHash> GetBlockHashAsync(long height, CancellationToken cancellationToken)
    {
        // return _blockChainService.Service.GetBlockHashAsync(height, cancellationToken);
        throw new NotImplementedException();
    }

    public async Task<T> GetActionAsync<T>(
        TxId txId,
        int actionIndex,
        CancellationToken cancellationToken)
        where T : IAction
    {
        // var bytes = await _blockChainService.Service.GetActionAsync(
        //     txId, actionIndex, cancellationToken);
        // var value = _codec.Decode(bytes);
        // if (Activator.CreateInstance(typeof(T)) is T action)
        // {
        //     action.LoadPlainValue(value);
        //     return action;
        // }

        // throw new InvalidOperationException("Action not found.");
        throw new NotImplementedException();
    }
}
