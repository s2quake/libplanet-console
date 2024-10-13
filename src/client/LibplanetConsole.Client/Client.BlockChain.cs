using System.Security.Cryptography;
using Grpc.Core;
using LibplanetConsole.Blockchain;
using LibplanetConsole.Blockchain.Grpc;
using LibplanetConsole.Node;

namespace LibplanetConsole.Client;

internal sealed partial class Client : IBlockChain
{
    private static readonly Codec _codec = new();

    public event EventHandler<BlockEventArgs>? BlockAppended;

    public BlockInfo Tip => Info.Tip;

    public async Task<TxId> SendTransactionAsync(
        IAction[] actions, CancellationToken cancellationToken)
    {
        if (_blockChainService is null)
        {
            throw new InvalidOperationException("BlockChainService is not initialized.");
        }

        var address = _privateKey.Address;
        var nonce = await GetNextNonceAsync(address, cancellationToken);
        var genesisHash = NodeInfo.GenesisHash;
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
        var callOptions = new CallOptions(
            cancellationToken: cancellationToken);
        var response = await _blockChainService.SendTransactionAsync(request, callOptions);
        return TxId.FromHex(response.TxId);
    }

    public async Task<TxId> SendTransactionAsync(
        byte[] txData, CancellationToken cancellationToken)
    {
        if (_blockChainService is null)
        {
            throw new InvalidOperationException("BlockChainService is not initialized.");
        }

        var request = new SendTransactionRequest
        {
            TransactionData = Google.Protobuf.ByteString.CopyFrom(txData),
        };
        var callOptions = new CallOptions(
            cancellationToken: cancellationToken);
        var response = await _blockChainService.SendTransactionAsync(request, callOptions);
        return TxId.FromHex(response.TxId);
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
        var options = new CallOptions(
            cancellationToken: cancellationToken);
        var response = await _blockChainService.GetBlockHashAsync(request, options);
        return BlockHash.FromString(response.BlockHash);
    }

    public async Task<long> GetNextNonceAsync(Address address, CancellationToken cancellationToken)
    {
        if (_blockChainService is null)
        {
            throw new InvalidOperationException("BlockChainService is not initialized.");
        }

        var request = new GetNextNonceRequest
        {
            Address = address.ToHex(),
        };
        var options = new CallOptions(
            cancellationToken: cancellationToken);
        var response = await _blockChainService.GetNextNonceAsync(request, options);
        return response.Nonce;
    }

    public async Task<BlockHash> GetTipHashAsync(CancellationToken cancellationToken)
    {
        if (_blockChainService is null)
        {
            throw new InvalidOperationException("BlockChainService is not initialized.");
        }

        var request = new GetTipHashRequest();
        var options = new CallOptions(
            cancellationToken: cancellationToken);
        var response = await _blockChainService.GetTipHashAsync(request, options);
        return BlockHash.FromString(response.BlockHash);
    }

    public async Task<IValue> GetStateAsync(
        BlockHash? blockHash,
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
            BlockHash = blockHash?.ToString() ?? string.Empty,
            AccountAddress = accountAddress.ToHex(),
            Address = address.ToHex(),
        };
        var options = new CallOptions(
            cancellationToken: cancellationToken);
        var response = await _blockChainService.GetStateAsync(request, options);
        return _codec.Decode(response.StateData.ToByteArray());
    }

    public async Task<IValue> GetStateByStateRootHashAsync(
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
            StateRootHash = stateRootHash.ToString(),
            AccountAddress = accountAddress.ToHex(),
            Address = address.ToHex(),
        };
        var options = new CallOptions(
            cancellationToken: cancellationToken);
        var response = await _blockChainService.GetStateAsync(request, options);
        return _codec.Decode(response.StateData.ToByteArray());
    }

    public async Task<byte[]> GetActionAsync(
        TxId txId, int actionIndex, CancellationToken cancellationToken)
    {
        if (_blockChainService is null)
        {
            throw new InvalidOperationException("BlockChainService is not initialized.");
        }

        var request = new GetActionRequest
        {
            TxId = txId.ToHex(),
            ActionIndex = actionIndex,
        };
        var options = new CallOptions(
            cancellationToken: cancellationToken);
        var response = await _blockChainService.GetActionAsync(request, options);
        return response.ActionData.ToByteArray();
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
            TxId = txId.ToHex(),
            ActionIndex = actionIndex,
        };
        var options = new CallOptions(
            cancellationToken: cancellationToken);
        var response = await _blockChainService.GetActionAsync(request, options);
        var value = _codec.Decode(response.ActionData.ToByteArray());
        if (Activator.CreateInstance(typeof(T)) is T action)
        {
            action.LoadPlainValue(value);
            return action;
        }

        throw new InvalidOperationException("Action not found.");
    }
}
