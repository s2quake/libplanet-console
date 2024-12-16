using Grpc.Core;
using LibplanetConsole.Grpc.Delegation;
using static LibplanetConsole.Grpc.TypeUtility;

namespace LibplanetConsole.Client.Delegation.Services;

internal sealed class DelegationServiceGrpcV1(IDelegation delegation)
    : DelegationGrpcService.DelegationGrpcServiceBase
{
    public override async Task<StakeResponse> Stake(StakeRequest request, ServerCallContext context)
    {
        var ncg = request.Ncg;
        await delegation.StakeAsync(ncg, context.CancellationToken);
        return new StakeResponse();
    }

    public override async Task<GetDelegatorInfoResponse> GetDelegatorInfo(
        GetDelegatorInfoRequest request, ServerCallContext context)
    {
        var address = ToAddress(request.Address);
        var delegatorInfo = await delegation.GetDelegatorInfoAsync(
            address, context.CancellationToken);
        return new GetDelegatorInfoResponse
        {
            DelegatorInfo = delegatorInfo,
        };
    }
}
