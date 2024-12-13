using Grpc.Core;
using LibplanetConsole.Grpc;
using LibplanetConsole.Grpc.Blockchain;
using Microsoft.Extensions.Hosting;
using static LibplanetConsole.Grpc.TypeUtility;

namespace LibplanetConsole.Client.Services;

internal sealed class BlockChainGrpcServiceV1(
    ClientBlockChain blockChain,
    IHostApplicationLifetime applicationLifetime)
    : BlockChainGrpcService.BlockChainGrpcServiceBase
{
    private static readonly Codec _codec = new();

    public async override Task<SendTransactionResponse> SendTransaction(
        SendTransactionRequest request, ServerCallContext context)
    {
        var txData = request.TransactionData.ToByteArray();
        var txId = await blockChain.SendTransactionAsync(txData, context.CancellationToken);
        return new SendTransactionResponse { TxId = ToGrpc(txId) };
    }

    public async override Task<GetNextNonceResponse> GetNextNonce(
        GetNextNonceRequest request, ServerCallContext context)
    {
        var address = ToAddress(request.Address);
        var nonce = await blockChain.GetNextNonceAsync(address, context.CancellationToken);
        return new GetNextNonceResponse { Nonce = nonce };
    }

    public override async Task<GetTipHashResponse> GetTipHash(
        GetTipHashRequest request, ServerCallContext context)
    {
        var blockHash = await Task.FromResult(blockChain.Tip.Hash);
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
                return await blockChain.GetStateAsync(
                    request.Height, accountAddress, address, cancellationToken);
            }

            if (request.IdentifierCase == GetStateRequest.IdentifierOneofCase.BlockHash)
            {
                var blockHash = ToBlockHash(request.BlockHash);
                return await blockChain.GetStateAsync(
                    blockHash, accountAddress, address, cancellationToken);
            }

            if (request.IdentifierCase == GetStateRequest.IdentifierOneofCase.StateRootHash)
            {
                var stateRootHash = ToHashDigest256(request.StateRootHash);
                return await blockChain.GetStateAsync(
                    stateRootHash, accountAddress, address, cancellationToken);
            }

            throw new NotSupportedException("Invalid IdentifierCase");
        }
    }

    public override async Task<GetBlockHashResponse> GetBlockHash(
        GetBlockHashRequest request, ServerCallContext context)
    {
        var height = request.Height;
        var blockHash = await blockChain.GetBlockHashAsync(height, context.CancellationToken);
        return new GetBlockHashResponse { BlockHash = ToGrpc(blockHash) };
    }

    public override async Task<GetActionResponse> GetAction(
        GetActionRequest request, ServerCallContext context)
    {
        var txId = ToTxId(request.TxId);
        var actionIndex = request.ActionIndex;
        var action = await blockChain.GetActionAsync(txId, actionIndex, context.CancellationToken);
        return new GetActionResponse { ActionData = Google.Protobuf.ByteString.CopyFrom(action) };
    }

    public override async Task GetEventStream(
        GetEventStreamRequest request,
        IServerStreamWriter<GetEventStreamResponse> responseStream,
        ServerCallContext context)
    {
        var streamer = new EventStreamer<GetEventStreamResponse>(responseStream)
        {
            Items =
            {
                new EventItem<GetEventStreamResponse, BlockEventArgs>
                {
                    Attach = handler => blockChain.BlockAppended += handler,
                    Detach = handler => blockChain.BlockAppended -= handler,
                    Selector = (e) => new GetEventStreamResponse
                    {
                        BlockAppended = new()
                        {
                            BlockInfo = e.BlockInfo,
                        },
                    },
                },
            },
        };

        await streamer.RunAsync(applicationLifetime, context.CancellationToken);
    }
}
