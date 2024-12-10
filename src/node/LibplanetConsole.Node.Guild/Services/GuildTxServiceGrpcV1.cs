using Grpc.Core;
using LibplanetConsole.Grpc.Guild;
using static LibplanetConsole.Grpc.TypeUtility;

namespace LibplanetConsole.Node.Guild.Services;

internal sealed class GuildTxServiceGrpcV1(IGuild guild, IBlockChain blockChain)
    : GuildTxGrpcService.GuildTxGrpcServiceBase
{
    public override Task<CreateTxResponse> Create(CreateTxRequest request, ServerCallContext context)
    {
        var nonce = request.Nonce;
        var validatorAddrses = ToAddress(request.ValidatorAddress);

        throw new NotImplementedException();
    }
}
