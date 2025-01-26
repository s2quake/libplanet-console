using System.Security.Cryptography;
using System.Text;
using Libplanet.Action.State;
using LibplanetConsole.Common.Exceptions;

namespace LibplanetConsole.Node;

internal sealed partial class Node : IBlockChain
{
    private static readonly Codec _codec = new();

    public event EventHandler<BlockEventArgs>? BlockAppended;

    public BlockInfo Tip => Info.Tip;

    public async Task<TxId> SendTransactionAsync(
        IAction[] actions, CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        if (IsRunning is false)
        {
            throw new InvalidOperationException("Node is not running.");
        }

        var privateKey = _privateKey;
        var blockChain = BlockChain;
        var genesisBlock = blockChain.Genesis;
        var nonce = blockChain.GetNextTxNonce(privateKey.Address);
        var values = actions.Select(item => item.PlainValue).ToArray();
        var transaction = Transaction.Create(
            nonce: nonce,
            privateKey: privateKey,
            genesisHash: genesisBlock.Hash,
            actions: new TxActionList(values));
        await SendTransactionAsync(transaction, cancellationToken);
        return transaction.Id;
    }

    public async Task SendTransactionAsync(
        Transaction transaction, CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        if (IsRunning is false)
        {
            throw new InvalidOperationException("Node is not running.");
        }

        _logger.LogDebug("Node adds a transaction: {TxId}", transaction.Id);
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

        _logger.LogDebug("Node added a transaction: {TxId}", transaction.Id);
    }

    public Task<long> GetNextNonceAsync(Address address, CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        if (IsRunning is false)
        {
            throw new InvalidOperationException("Node is not running.");
        }

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
        if (IsRunning is false)
        {
            throw new InvalidOperationException("Node is not running.");
        }

        BlockHash GetTipHash()
        {
            var blockChain = BlockChain;
            return blockChain.Tip.Hash;
        }

        return Task.Run(GetTipHash, cancellationToken);
    }

    public Task<IValue> GetStateAsync(
        long height,
        Address accountAddress,
        Address address,
        CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        if (IsRunning is false)
        {
            throw new InvalidOperationException("Node is not running.");
        }

        IValue GetStateByBlockHash()
        {
            var worldState = GetWorldState();
            var account = worldState.GetAccountState(accountAddress);
            return account.GetState(address)
                ?? throw new InvalidOperationException(
                    $"Account '{accountAddress}' does not have state '{address}'.");
        }

        IWorldState GetWorldState()
        {
            var blockChain = BlockChain;
            if (height == -1 && blockChain.GetNextWorldState() is { } nextState)
            {
                return nextState;
            }

            var block = height == -1 ? blockChain.Tip : blockChain[height];
            return blockChain.GetWorldState(block.Hash);
        }

        return Task.Run(GetStateByBlockHash, cancellationToken: cancellationToken);
    }

    public Task<IValue> GetStateAsync(
        BlockHash blockHash,
        Address accountAddress,
        Address address,
        CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        if (IsRunning is false)
        {
            throw new InvalidOperationException("Node is not running.");
        }

        IValue GetStateByBlockHash()
        {
            var blockChain = BlockChain;
            var worldState = blockChain.GetWorldState(blockHash);
            var account = worldState.GetAccountState(accountAddress);
            return account.GetState(address)
                ?? throw new InvalidOperationException("State not found.");
        }

        return Task.Run(GetStateByBlockHash, cancellationToken: cancellationToken);
    }

    public Task<IValue> GetStateAsync(
        HashDigest<SHA256> stateRootHash,
        Address accountAddress,
        Address address,
        CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        if (IsRunning is false)
        {
            throw new InvalidOperationException("Node is not running.");
        }

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
        if (IsRunning is false)
        {
            throw new InvalidOperationException("Node is not running.");
        }

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

    public Task<AddressInfo[]> GetAddressesAsync(CancellationToken cancellationToken)
    {
        AddressInfo[] GetAddresses()
        {
            var addresses = _serviceProvider.GetRequiredService<AddressCollection>();
            return addresses.GetAddressInfos();
        }

        return Task.Run(GetAddresses, cancellationToken);
    }

    public Task<FungibleAssetValue> GetBalanceAsync(
        Address address, Currency currency, CancellationToken cancellationToken)
    {
        FungibleAssetValue GetBalance()
        {
            var worldState = GetWorldState();
            return worldState.GetBalance(address, currency);
        }

        return Task.Run(GetBalance, cancellationToken);
    }

    public IWorldState GetWorldState() => BlockChain.GetNextWorldState()
        ?? BlockChain.GetWorldState(BlockChain.Tip.Hash);

    public IWorldState GetWorldState(BlockHash offset) => BlockChain.GetWorldState(offset);

    public IWorldState GetWorldState(long height)
        => BlockChain.GetWorldState(BlockChain[height].Hash);
}
