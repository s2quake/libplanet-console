using Grpc.Core;
using LibplanetConsole.Node.Grpc;

namespace LibplanetConsole.Node.Services;

internal sealed class BlockChainGrpcServiceV1(Node node, IBlockChain blockChain)
    : BlockChainGrpcService.BlockChainGrpcServiceBase
{
    private static readonly Codec _codec = new();

    public async override Task<SendTransactionResponse> SendTransaction(
        SendTransactionRequest request, ServerCallContext context)
    {
        var tx = Transaction.Deserialize(request.TransactionData.ToByteArray());
        await node.AddTransactionAsync(tx, context.CancellationToken);
        return new SendTransactionResponse { TxId = tx.Id.ToHex() };
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
        var action = await node.GetActionAsync(txId, actionIndex, context.CancellationToken);
        return new GetActionResponse { ActionData = Google.Protobuf.ByteString.CopyFrom(action) };
    }
}
