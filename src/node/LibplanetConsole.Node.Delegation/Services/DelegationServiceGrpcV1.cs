using Grpc.Core;
using LibplanetConsole.Grpc.Delegation;
using static LibplanetConsole.Grpc.TypeUtility;

namespace LibplanetConsole.Node.Delegation.Services;

internal sealed class DelegationServiceGrpcV1(Delegation delegation)
    : DelegationGrpcService.DelegationGrpcServiceBase
{
    public override async Task<StakeResponse> Stake(StakeRequest request, ServerCallContext context)
    {
        var amount = request.Amount;
        await delegation.StakeAsync(amount, context.CancellationToken);
        return new StakeResponse();
    }
}
