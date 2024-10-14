using System.Security.Cryptography;
using Grpc.Core;
using LibplanetConsole.Blockchain;
using LibplanetConsole.Blockchain.Grpc;
using static LibplanetConsole.Blockchain.Grpc.TypeUtility;

namespace LibplanetConsole.Console;

internal sealed partial class Client : IBlockChain
{
    private static readonly Codec _codec = new();

    public event EventHandler<BlockEventArgs>? BlockAppended;

    public BlockInfo Tip => Info.Tip;

    public async Task<long> GetNextNonceAsync(Address address, CancellationToken cancellationToken)
    {
        if (_blockChainService is null)
        {
            throw new InvalidOperationException("BlockChainService is not initialized.");
        }

        var request = new GetNextNonceRequest
        {
            Address = ToGrpc(address),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _blockChainService.GetNextNonceAsync(request, callOptions);
        return response.Nonce;
    }

    public async Task<TxId> SendTransactionAsync(
        IAction[] actions, CancellationToken cancellationToken)
    {
        if (_blockChainService is null)
        {
            throw new InvalidOperationException("BlockChainService is not initialized.");
        }

        var address = _privateKey.Address;
        var nonce = await GetNextNonceAsync(address, cancellationToken);
        var genesisHash = _clientInfo.GenesisHash;
        var tx = Transaction.Create(
            nonce: nonce,
            privateKey: _privateKey,
            genesisHash: genesisHash,
            actions: [.. actions.Select(item => item.PlainValue)]);
        var txData = tx.Serialize();
        var request = new SendTransactionRequest
        {
            TransactionData = Google.Protobuf.ByteString.CopyFrom(txData),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _blockChainService.SendTransactionAsync(request, callOptions);
        return ToTxId(response.TxId);
    }

    public async Task<BlockHash> GetTipHashAsync(CancellationToken cancellationToken)
    {
        if (_blockChainService is null)
        {
            throw new InvalidOperationException("BlockChainService is not initialized.");
        }

        var request = new GetTipHashRequest();
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _blockChainService.GetTipHashAsync(request, callOptions);
        return ToBlockHash(response.BlockHash);
    }

    public async Task<IValue> GetStateAsync(
        long height,
        Address accountAddress,
        Address address,
        CancellationToken cancellationToken)
    {
        if (_blockChainService is null)
        {
            throw new InvalidOperationException("BlockChainService is not initialized.");
        }

        var request = new GetStateRequest
        {
            Height = height,
            AccountAddress = ToGrpc(accountAddress),
            Address = ToGrpc(address),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _blockChainService.GetStateAsync(request, callOptions);
        return _codec.Decode(response.StateData.ToByteArray());
    }

    public async Task<IValue> GetStateAsync(
        BlockHash blockHash,
        Address accountAddress,
        Address address,
        CancellationToken cancellationToken)
    {
        if (_blockChainService is null)
        {
            throw new InvalidOperationException("BlockChainService is not initialized.");
        }

        var request = new GetStateRequest
        {
            BlockHash = ToGrpc(blockHash),
            AccountAddress = ToGrpc(accountAddress),
            Address = ToGrpc(address),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _blockChainService.GetStateAsync(request, callOptions);
        return _codec.Decode(response.StateData.ToByteArray());
    }

    public async Task<IValue> GetStateAsync(
        HashDigest<SHA256> stateRootHash,
        Address accountAddress,
        Address address,
        CancellationToken cancellationToken)
    {
        if (_blockChainService is null)
        {
            throw new InvalidOperationException("BlockChainService is not initialized.");
        }

        var request = new GetStateRequest
        {
            StateRootHash = ToGrpc(stateRootHash),
            AccountAddress = ToGrpc(accountAddress),
            Address = ToGrpc(address),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _blockChainService.GetStateAsync(request, callOptions);
        return _codec.Decode(response.StateData.ToByteArray());
    }

    public async Task<BlockHash> GetBlockHashAsync(long height, CancellationToken cancellationToken)
    {
        if (_blockChainService is null)
        {
            throw new InvalidOperationException("BlockChainService is not initialized.");
        }

        var request = new GetBlockHashRequest
        {
            Height = height,
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _blockChainService.GetBlockHashAsync(request, callOptions);
        return ToBlockHash(response.BlockHash);
    }

    public async Task<T> GetActionAsync<T>(
        TxId txId, int actionIndex, CancellationToken cancellationToken)
        where T : IAction
    {
        if (_blockChainService is null)
        {
            throw new InvalidOperationException("BlockChainService is not initialized.");
        }

        var request = new GetActionRequest
        {
            TxId = ToGrpc(txId),
            ActionIndex = actionIndex,
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _blockChainService.GetActionAsync(request, callOptions);
        var value = _codec.Decode(response.ActionData.ToByteArray());
        if (Activator.CreateInstance(typeof(T)) is T action)
        {
            action.LoadPlainValue(value);
            return action;
        }

        throw new InvalidOperationException("Action not found.");
    }
}
