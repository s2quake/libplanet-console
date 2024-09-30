using System.Security.Cryptography;
using System.Text;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Exceptions;

namespace LibplanetConsole.Node;

internal sealed partial class Node : IBlockChain
{
    private static readonly Codec _codec = new();

    public async Task<TxId> AddTransactionAsync(
        IAction[] actions, CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(
            condition: IsRunning != true,
            message: "Node is not running.");

        var privateKey = PrivateKeyUtility.FromSecureString(_privateKey);
        var blockChain = BlockChain;
        var genesisBlock = blockChain.Genesis;
        var nonce = blockChain.GetNextTxNonce(privateKey.Address);
        var values = actions.Select(item => item.PlainValue).ToArray();
        var transaction = Transaction.Create(
            nonce: nonce,
            privateKey: privateKey,
            genesisHash: genesisBlock.Hash,
            actions: new TxActionList(values));
        await AddTransactionAsync(transaction, cancellationToken);
        return transaction.Id;
    }

    public async Task AddTransactionAsync(
        Transaction transaction, CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(
            condition: IsRunning != true,
            message: "Node is not running.");

        _logger.Debug("Node adds a transaction: {TxId}", transaction.Id);
        var blockChain = BlockChain;
        var manualResetEvent = _eventByTxId.GetOrAdd(transaction.Id, _ =>
        {
            return new ManualResetEvent(initialState: false);
        });
        blockChain.StageTransaction(transaction);
        await Task.Run(manualResetEvent.WaitOne, cancellationToken);

        _eventByTxId.TryRemove(transaction.Id, out _);

        var sb = new StringBuilder();
        foreach (var item in transaction.Actions)
        {
            if (_exceptionByAction.TryRemove(item, out var exception) == true &&
                exception is UnexpectedlyTerminatedActionException)
            {
                sb.AppendLine($"{exception.InnerException}");
            }
        }

        if (sb.Length > 0)
        {
            throw new InvalidOperationException(sb.ToString());
        }

        _logger.Debug("Node added a transaction: {TxId}", transaction.Id);
    }

    public Task<long> GetNextNonceAsync(Address address, CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(
            condition: IsRunning != true,
            message: "Node is not running.");

        return Task.Run(GetNextNonce, cancellationToken);

        long GetNextNonce()
        {
            var blockChain = BlockChain;
            var nonce = blockChain.GetNextTxNonce(address);
            return nonce;
        }
    }

    public Task<BlockHash> GetTipHashAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(
            condition: IsRunning != true,
            message: "Node is not running.");

        BlockHash GetTipHash()
        {
            var blockChain = BlockChain;
            return blockChain.Tip.Hash;
        }

        return Task.Run(GetTipHash, cancellationToken);
    }

    public Task<IValue> GetStateAsync(
        BlockHash? blockHash,
        Address accountAddress,
        Address address,
        CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(
            condition: IsRunning != true,
            message: "Node is not running.");

        IValue GetStateByBlockHash()
        {
            var blockChain = BlockChain;
            var block = blockHash is null ? blockChain.Tip : blockChain[blockHash.Value];
            var isTip = block.Hash.Equals(blockChain.Tip.Hash);
            var worldState = isTip
                ? blockChain.GetNextWorldState() ?? blockChain.GetWorldState(block.Hash)
                : blockChain.GetWorldState(block.Hash);
            var account = worldState.GetAccountState(accountAddress);
            return account.GetState(address)
                ?? throw new InvalidOperationException("State not found.");
        }

        return Task.Run(GetStateByBlockHash, cancellationToken: cancellationToken);
    }

    public Task<IValue> GetStateByStateRootHashAsync(
        HashDigest<SHA256> stateRootHash,
        Address accountAddress,
        Address address,
        CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(
            condition: IsRunning != true,
            message: "Node is not running.");

        IValue GetStateByStateRootHash()
        {
            var blockChain = BlockChain;
            var worldState = blockChain.GetWorldState(stateRootHash);
            var account = worldState.GetAccountState(accountAddress);
            return account.GetState(address)
                ?? throw new InvalidOperationException("State not found.");
        }

        return Task.Run(GetStateByStateRootHash, cancellationToken);
    }

    public Task<BlockHash> GetBlockHashAsync(long height, CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(
            condition: IsRunning != true,
            message: "Node is not running.");

        BlockHash GetBlockHash()
        {
            var blockChain = BlockChain;
            var block = blockChain[height];
            return block.Hash;
        }

        return Task.Run(GetBlockHash, cancellationToken);
    }

    public Task<byte[]> GetActionAsync(
        TxId txId, int actionIndex, CancellationToken cancellationToken)
    {
        byte[] GetAction()
        {
            var blockChain = BlockChain;
            var transaction = blockChain.GetTransaction(txId);
            var action = transaction.Actions[actionIndex];
            return _codec.Encode(action);
        }

        return Task.Run(GetAction, cancellationToken);
    }

    public Task<T> GetActionAsync<T>(
        TxId txId, int actionIndex, CancellationToken cancellationToken)
        where T : IAction
    {
        T GetAction()
        {
            var blockChain = BlockChain;
            var transaction = blockChain.GetTransaction(txId);
            var value = transaction.Actions[actionIndex];
            if (Activator.CreateInstance(typeof(T)) is T action)
            {
                action.LoadPlainValue(value);
                return action;
            }

            throw new InvalidOperationException("Action not found.");
        }

        return Task.Run(GetAction, cancellationToken);
    }
}
