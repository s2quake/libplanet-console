using Grpc.Core;
using LibplanetConsole.Delegation.Grpc;
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

    public override async Task<GetDelegateeInfoResponse> GetDelegateeInfo(
        GetDelegateeInfoRequest request, ServerCallContext context)
    {
        var address = ToAddress(request.Address);
        var delegateeInfo = await delegation.GetDelegateeInfoAsync(
            address, context.CancellationToken);
        return new GetDelegateeInfoResponse
        {
            DelegateeInfo = delegateeInfo,
        };
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

    public override async Task<GetStakeInfoResponse> GetStakeInfo(
        GetStakeInfoRequest request, ServerCallContext context)
    {
        var address = ToAddress(request.Address);
        var delegatorInfo = await delegation.GetStakeInfoAsync(
            address, context.CancellationToken);
        return new GetStakeInfoResponse
        {
            StakeInfo = delegatorInfo,
        };
    }
}
