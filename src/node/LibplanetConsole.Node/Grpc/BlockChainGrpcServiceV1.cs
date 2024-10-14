using Grpc.Core;
using LibplanetConsole.Blockchain;
using LibplanetConsole.Blockchain.Grpc;
using LibplanetConsole.Grpc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static LibplanetConsole.Blockchain.Grpc.TypeUtility;

namespace LibplanetConsole.Node.Grpc;

internal sealed class BlockChainGrpcServiceV1 : BlockChainGrpcService.BlockChainGrpcServiceBase
{
    private static readonly Codec _codec = new();
    private readonly Node _node;
    private readonly IBlockChain _blockChain;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly ILogger<BlockChainGrpcServiceV1> _logger;

    public BlockChainGrpcServiceV1(
        Node node,
        IBlockChain blockChain,
        IHostApplicationLifetime applicationLifetime,
        ILogger<BlockChainGrpcServiceV1> logger)
    {
        _node = node;
        _blockChain = blockChain;
        _applicationLifetime = applicationLifetime;
        _logger = logger;
        _logger.LogDebug("BlockChainGrpcServiceV1 is created.");
    }

    public async override Task<SendTransactionResponse> SendTransaction(
        SendTransactionRequest request, ServerCallContext context)
    {
        _logger.LogDebug("{MethodName} is called.", nameof(SendTransaction));
        var tx = Transaction.Deserialize(request.TransactionData.ToByteArray());
        await _node.SendTransactionAsync(tx, context.CancellationToken);
        return new SendTransactionResponse { TxId = ToGrpc(tx.Id) };
    }

    public async override Task<GetNextNonceResponse> GetNextNonce(
        GetNextNonceRequest request, ServerCallContext context)
    {
        var address = ToAddress(request.Address);
        var nonce = await _blockChain.GetNextNonceAsync(address, context.CancellationToken);
        return new GetNextNonceResponse { Nonce = nonce };
    }

    public override async Task<GetTipHashResponse> GetTipHash(
        GetTipHashRequest request, ServerCallContext context)
    {
        var blockHash = await Task.FromResult(_blockChain.Tip.Hash);
        return new GetTipHashResponse { BlockHash = ToGrpc(blockHash) };
    }

    public override async Task<GetStateResponse> GetState(
        GetStateRequest request, ServerCallContext context)
    {
        var accountAddress = ToAddress(request.AccountAddress);
        var address = ToAddress(request.Address);
        var value = await GetStateAsync(request, context.CancellationToken);

        var state = _codec.Encode(value);
        return new GetStateResponse { StateData = Google.Protobuf.ByteString.CopyFrom(state) };

        async Task<IValue> GetStateAsync(
            GetStateRequest request, CancellationToken cancellationToken)
        {
            if (request.IdentifierCase == GetStateRequest.IdentifierOneofCase.Height)
            {
                return await _blockChain.GetStateAsync(
                    request.Height, accountAddress, address, cancellationToken);
            }

            if (request.IdentifierCase == GetStateRequest.IdentifierOneofCase.BlockHash)
            {
                var blockHash = ToBlockHash(request.BlockHash);
                return await _blockChain.GetStateAsync(
                    blockHash, accountAddress, address, cancellationToken);
            }

            if (request.IdentifierCase == GetStateRequest.IdentifierOneofCase.StateRootHash)
            {
                var stateRootHash = ToHashDigest256(request.StateRootHash);
                return await _blockChain.GetStateAsync(
                    stateRootHash, accountAddress, address, cancellationToken);
            }

            throw new NotSupportedException("Invalid IdentifierCase");
        }
    }

    public override async Task<GetBlockHashResponse> GetBlockHash(
        GetBlockHashRequest request, ServerCallContext context)
    {
        var height = request.Height;
        var blockHash = await _blockChain.GetBlockHashAsync(height, context.CancellationToken);
        return new GetBlockHashResponse { BlockHash = ToGrpc(blockHash) };
    }

    public override async Task<GetActionResponse> GetAction(
        GetActionRequest request, ServerCallContext context)
    {
        var txId = ToTxId(request.TxId);
        var actionIndex = request.ActionIndex;
        var action = await _node.GetActionAsync(txId, actionIndex, context.CancellationToken);
        return new GetActionResponse { ActionData = Google.Protobuf.ByteString.CopyFrom(action) };
    }

    public override async Task GetBlockAppendedStream(
        GetBlockAppendedStreamRequest request,
        IServerStreamWriter<GetBlockAppendedStreamResponse> responseStream,
        ServerCallContext context)
    {
        var streamer = new EventStreamer<GetBlockAppendedStreamResponse, BlockEventArgs>(
            responseStream,
            attach: handler => _blockChain.BlockAppended += handler,
            detach: handler => _blockChain.BlockAppended -= handler,
            selector: e => new GetBlockAppendedStreamResponse { BlockInfo = e.BlockInfo });
        await streamer.RunAsync(_applicationLifetime, context.CancellationToken);
    }
}
