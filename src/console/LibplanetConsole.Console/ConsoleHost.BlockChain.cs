using System.Security.Cryptography;

namespace LibplanetConsole.Console;

internal sealed partial class ConsoleHost : IBlockChain
{
    public event EventHandler<BlockEventArgs>? BlockAppended;

    Task<long> IBlockChain.GetNextNonceAsync(
        Address address, CancellationToken cancellationToken)
    {
        if (IsRunning is false || _node is null)
        {
            throw new InvalidOperationException("BlockChain is not running.");
        }

        return _node.GetNextNonceAsync(address, cancellationToken);
    }

    Task<IValue> IBlockChain.GetStateAsync(
        long height,
        Address accountAddress,
        Address address,
        CancellationToken cancellationToken)
    {
        if (IsRunning is false || _node is null)
        {
            throw new InvalidOperationException("BlockChain is not running.");
        }

        return _node.GetStateAsync(height, accountAddress, address, cancellationToken);
    }

    Task<IValue> IBlockChain.GetStateAsync(
        BlockHash blockHash,
        Address accountAddress,
        Address address,
        CancellationToken cancellationToken)
    {
        if (IsRunning is false || _node is null)
        {
            throw new InvalidOperationException("BlockChain is not running.");
        }

        return _node.GetStateAsync(blockHash, accountAddress, address, cancellationToken);
    }

    Task<IValue> IBlockChain.GetStateAsync(
        HashDigest<SHA256> stateRootHash,
        Address accountAddress,
        Address address,
        CancellationToken cancellationToken)
    {
        if (IsRunning is false || _node is null)
        {
            throw new InvalidOperationException("BlockChain is not running.");
        }

        return _node.GetStateAsync(
            stateRootHash, accountAddress, address, cancellationToken);
    }

    Task<BlockHash> IBlockChain.GetBlockHashAsync(
        long height, CancellationToken cancellationToken)
    {
        if (IsRunning is false || _node is null)
        {
            throw new InvalidOperationException("BlockChain is not running.");
        }

        return _node.GetBlockHashAsync(height, cancellationToken);
    }

    Task<T> IBlockChain.GetActionAsync<T>(
        TxId txId, int actionIndex, CancellationToken cancellationToken)
    {
        if (IsRunning is false || _node is null)
        {
            throw new InvalidOperationException("BlockChain is not running.");
        }

        return _node.GetActionAsync<T>(txId, actionIndex, cancellationToken);
    }
}
