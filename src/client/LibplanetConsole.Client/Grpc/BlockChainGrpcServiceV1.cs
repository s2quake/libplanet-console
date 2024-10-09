using Grpc.Core;
using LibplanetConsole.Blockchain;
using LibplanetConsole.Blockchain.Grpc;
using LibplanetConsole.Grpc;
using Microsoft.Extensions.Hosting;

namespace LibplanetConsole.Client.Grpc;

internal sealed class BlockChainGrpcServiceV1(
    Client client,
    IBlockChain blockChain,
    IHostApplicationLifetime applicationLifetime)
    : BlockChainGrpcService.BlockChainGrpcServiceBase
{
    private static readonly Codec _codec = new();

    public async override Task<SendTransactionResponse> SendTransaction(
        SendTransactionRequest request, ServerCallContext context)
    {
        var txData = request.TransactionData.ToByteArray();
        var txId = await client.SendTransactionAsync(txData, context.CancellationToken);
        return new SendTransactionResponse { TxId = txId.ToHex() };
    }

    public async override Task<GetNextNonceResponse> GetNextNonce(
        GetNextNonceRequest request, ServerCallContext context)
    {
        var address = new Address(request.Address);
        var nonce = await blockChain.GetNextNonceAsync(address, context.CancellationToken);
        return new GetNextNonceResponse { Nonce = nonce };
    }

    public override async Task<GetTipHashResponse> GetTipHash(
        GetTipHashRequest request, ServerCallContext context)
    {
        var blockHash = await blockChain.GetTipHashAsync(context.CancellationToken);
        return new GetTipHashResponse { BlockHash = blockHash.ToString() };
    }

    public override async Task<GetStateResponse> GetState(
        GetStateRequest request, ServerCallContext context)
    {
        BlockHash? blockHash = request.BlockHash == string.Empty
            ? null : BlockHash.FromString(request.BlockHash);
        var accountAddress = new Address(request.AccountAddress);
        var address = new Address(request.Address);
        var value = await blockChain.GetStateAsync(
            blockHash, accountAddress, address, context.CancellationToken);
        var state = _codec.Encode(value);
        return new GetStateResponse { StateData = Google.Protobuf.ByteString.CopyFrom(state) };
    }

    public override async Task<GetBlockHashResponse> GetBlockHash(
        GetBlockHashRequest request, ServerCallContext context)
    {
        var height = request.Height;
        var blockHash = await blockChain.GetBlockHashAsync(height, context.CancellationToken);
        return new GetBlockHashResponse { BlockHash = blockHash.ToString() };
    }

    public override async Task<GetActionResponse> GetAction(
        GetActionRequest request, ServerCallContext context)
    {
        var txId = TxId.FromHex(request.TxId);
        var actionIndex = request.ActionIndex;
        var action = await client.GetActionAsync(txId, actionIndex, context.CancellationToken);
        return new GetActionResponse { ActionData = Google.Protobuf.ByteString.CopyFrom(action) };
    }

    public override async Task GetBlockAppendedStream(
        GetBlockAppendedStreamRequest request,
        IServerStreamWriter<GetBlockAppendedStreamResponse> responseStream,
        ServerCallContext context)
    {
        var streamer = new EventStreamer<GetBlockAppendedStreamResponse, BlockEventArgs>(
            responseStream,
            attach: handler => blockChain.BlockAppended += handler,
            detach: handler => blockChain.BlockAppended -= handler,
            selector: e => new GetBlockAppendedStreamResponse { BlockInfo = e.BlockInfo });
        await streamer.RunAsync(applicationLifetime, context.CancellationToken);
    }
}
