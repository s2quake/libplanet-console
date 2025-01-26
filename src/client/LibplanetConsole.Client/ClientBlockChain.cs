using System.Security.Cryptography;
using Grpc.Core;
using Grpc.Net.Client;
using LibplanetConsole.Client.Services;
using LibplanetConsole.Common;
using LibplanetConsole.Grpc.BlockChain;
using static LibplanetConsole.Grpc.TypeUtility;

namespace LibplanetConsole.Client;

internal sealed class ClientBlockChain(Client client) : ClientContentBase, IBlockChain
{
    private static readonly Codec _codec = new();
    private GrpcChannel? _channel;
    private BlockChainService? _blockChainService;

    public event EventHandler<BlockEventArgs>? BlockAppended;

    public event EventHandler? Started;

    public event EventHandler? Stopped;

    public BlockInfo Tip { get; private set; } = BlockInfo.Empty;

    public async Task<TxId> SendTransactionAsync(
        Transaction transaction, CancellationToken cancellationToken)
    {
        if (_blockChainService is null)
        {
            throw new InvalidOperationException("BlockChainService is not initialized.");
        }

        var txData = transaction.Serialize();
        var request = new SendTransactionRequest
        {
            TransactionData = Google.Protobuf.ByteString.CopyFrom(txData),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _blockChainService.SendTransactionAsync(request, callOptions);
        return ToTxId(response.TxId);
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
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _blockChainService.SendTransactionAsync(request, callOptions);
        return ToTxId(response.TxId);
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

    public async Task<byte[]> GetActionAsync(
        TxId txId, int actionIndex, CancellationToken cancellationToken)
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

    public async Task<AddressInfo[]> GetAddressesAsync(CancellationToken cancellationToken)
    {
        if (_blockChainService is null)
        {
            throw new InvalidOperationException("BlockChainService is not initialized.");
        }

        var request = new GetAddressesRequest();
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _blockChainService.GetAddressesAsync(request, callOptions);
        return [.. response.AddressInfos.Select(item => (AddressInfo)item)];
    }

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        var address = $"http://{EndPointUtility.ToString(client.NodeEndPoint)}";
        var channel = GrpcChannel.ForAddress(address);
        var blockChainService = new BlockChainService(channel);
        _channel = channel;
        _blockChainService = blockChainService;
        await _blockChainService.InitializeAsync(cancellationToken);
        _blockChainService.BlockAppended += BlockChainService_BlockAppended;
        Started?.Invoke(this, EventArgs.Empty);
    }

    protected override async Task OnStopAsync(CancellationToken cancellationToken)
    {
        if (_blockChainService is not null)
        {
            await _blockChainService.ReleaseAsync(cancellationToken);
            _blockChainService = null;
        }

        if (_channel is not null)
        {
            await _channel.ShutdownAsync();
            _channel.Dispose();
            _channel = null;
        }

        await Task.CompletedTask;
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    private void BlockChainService_BlockAppended(object? sender, BlockEventArgs e)
    {
        Tip = e.BlockInfo;
        BlockAppended?.Invoke(this, e);
    }
}
