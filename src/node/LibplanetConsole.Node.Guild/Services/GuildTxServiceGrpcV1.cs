using Google.Protobuf;
using Grpc.Core;
using Lib9c;
using LibplanetConsole.Grpc.Guild;
using Nekoyume.Action.Guild;
using static LibplanetConsole.Grpc.TypeUtility;

namespace LibplanetConsole.Node.Guild.Services;

internal sealed class GuildTxServiceGrpcV1(
    IGuild guild, IBlockChain blockChain, IApplicationOptions options)
    : GuildTxGrpcService.GuildTxGrpcServiceBase
{
    private static readonly Codec _codec = new();

    public override Task<CreateTxResponse> Create(
        CreateTxRequest request, ServerCallContext context)
    {
        var validatorAddrses = ToAddress(request.ValidatorAddress);
        var makeGuild = new MakeGuild(validatorAddrses);
        var value = _codec.Encode(makeGuild.PlainValue);

        return Task.FromResult(new CreateTxResponse
        {
            PlainValue = ByteString.CopyFrom(value),
        });
    }

    public override Task<ClaimTxResponse> Claim(ClaimTxRequest request, ServerCallContext context)
    {
        var claimReward = new ClaimReward();
        var value = _codec.Encode(claimReward.PlainValue);

        return Task.FromResult(new ClaimTxResponse
        {
            PlainValue = ByteString.CopyFrom(value),
        });
    }
}
