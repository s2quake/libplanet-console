using System.Security.Cryptography;
using System.Text;
using Bencodex;
using Libplanet.Action;
using Libplanet.Common;
using Libplanet.Crypto;
using Libplanet.Types.Blocks;
using Libplanet.Types.Tx;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Exceptions;

namespace LibplanetConsole.Nodes;

internal sealed partial class Node : IBlockChain
{
    private static readonly Codec _codec = new();

    public async Task<AppId> AddTransactionAsync(
        IAction[] actions, CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(
            condition: IsRunning != true,
            message: "Node is not running.");

        var privateKey = AppPrivateKey.FromSecureString(_privateKey);
        var blockChain = BlockChain;
        var genesisBlock = blockChain.Genesis;
        var nonce = blockChain.GetNextTxNonce((Address)privateKey.Address);
        var values = actions.Select(item => item.PlainValue).ToArray();
        var transaction = Transaction.Create(
            nonce: nonce,
            privateKey: (PrivateKey)privateKey,
            genesisHash: genesisBlock.Hash,
            actions: new TxActionList(values));
        await AddTransactionAsync(transaction, cancellationToken);
        return (AppId)transaction.Id;
    }

    public async Task AddTransactionAsync(
        Transaction transaction, CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(
            condition: IsRunning != true,
            message: "Node is not running.");

        _logger.Debug("Node adds a transaction: {AppId}", transaction.Id);
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

        _logger.Debug("Node added a transaction: {AppId}", transaction.Id);
    }

    public Task<long> GetNextNonceAsync(AppAddress address, CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(
            condition: IsRunning != true,
            message: "Node is not running.");

        return Task.Run(GetNextNonce, cancellationToken);

        long GetNextNonce()
        {
            var blockChain = BlockChain;
            var nonce = blockChain.GetNextTxNonce((Address)address);
            return nonce;
        }
    }

    public Task<AppHash> GetTipHashAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(
            condition: IsRunning != true,
            message: "Node is not running.");

        AppHash GetTipHash()
        {
            var blockChain = BlockChain;
            return (AppHash)blockChain.Tip.Hash;
        }

        return Task.Run(GetTipHash, cancellationToken);
    }

    public Task<byte[]> GetStateByBlockHashAsync(
        AppHash blockHash,
        AppAddress accountAddress,
        AppAddress address,
        CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(
            condition: IsRunning != true,
            message: "Node is not running.");

        byte[] GetStateByBlockHash()
        {
            var blockChain = BlockChain;
            var worldState = blockChain.GetWorldState((BlockHash)blockHash);
            var account = worldState.GetAccountState((Address)accountAddress);
            var value = account.GetState((Address)address);
            if (value is not null)
            {
                return _codec.Encode(value);
            }

            return [];
        }

        return Task.Run(GetStateByBlockHash, cancellationToken: cancellationToken);
    }

    public Task<byte[]> GetStateByStateRootHashAsync(
        AppHash stateRootHash,
        AppAddress accountAddress,
        AppAddress address,
        CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(
            condition: IsRunning != true,
            message: "Node is not running.");

        byte[] GetStateByStateRootHash()
        {
            var blockChain = BlockChain;
            var worldState = blockChain.GetWorldState((HashDigest<SHA256>)stateRootHash);
            var account = worldState.GetAccountState((Address)accountAddress);
            var value = account.GetState((Address)address);
            if (value is not null)
            {
                return _codec.Encode(value);
            }

            return [];
        }

        return Task.Run(GetStateByStateRootHash, cancellationToken);
    }

    public Task<AppHash> GetBlockHashAsync(long height, CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(
            condition: IsRunning != true,
            message: "Node is not running.");

        AppHash GetBlockHash()
        {
            var blockChain = BlockChain;
            var block = blockChain[height];
            return (AppHash)block.Hash;
        }

        return Task.Run(GetBlockHash, cancellationToken);
    }
}
