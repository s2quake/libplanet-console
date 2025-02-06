using Grpc.Core;
using LibplanetConsole.Bank;
using LibplanetConsole.Common;
using LibplanetConsole.Delegation.Grpc;
using static LibplanetConsole.Grpc.TypeUtility;

namespace LibplanetConsole.Node.Delegation.Services;

internal sealed class DelegationServiceGrpcV1(
    IDelegation delegation, ICurrencyCollection currencies)
    : DelegationGrpcService.DelegationGrpcServiceBase
{
    public override async Task<StakeResponse> Stake(StakeRequest request, ServerCallContext context)
    {
        var ncg = request.Ncg;
        await delegation.StakeAsync(ncg, context.CancellationToken);
        return new StakeResponse();
    }

    public override async Task<PromoteResponse> Promote(
        PromoteRequest request, ServerCallContext context)
    {
        var guildGold = currencies.ToFungibleAssetValue(request.GuildGold);
        await delegation.PromoteAsync(guildGold, context.CancellationToken);
        return new PromoteResponse();
    }

    public override async Task<UnjailResponse> Unjail(
        UnjailRequest request, ServerCallContext context)
    {
        await delegation.UnjailAsync(context.CancellationToken);
        return new UnjailResponse();
    }

    public override async Task<DelegateResponse> Delegate(
        DelegateRequest request, ServerCallContext context)
    {
        var guildGold = currencies.ToFungibleAssetValue(request.GuildGold);
        await delegation.DelegateAsync(guildGold, context.CancellationToken);
        return new DelegateResponse();
    }

    public override async Task<UndelegateResponse> Undelegate(
        UndelegateRequest request, ServerCallContext context)
    {
        var share = BigIntegerUtility.Parse(request.Share);
        await delegation.UndelegateAsync(share, context.CancellationToken);
        return new UndelegateResponse();
    }

    public override async Task<SetCommissionResponse> SetCommission(
        SetCommissionRequest request, ServerCallContext context)
    {
        var commission = request.Commission;
        await delegation.SetCommissionAsync(commission, context.CancellationToken);
        return new SetCommissionResponse();
    }

    public override async Task<ClaimResponse> Claim(
        ClaimRequest request, ServerCallContext context)
    {
        await delegation.ClaimAsync(context.CancellationToken);
        return new ClaimResponse();
    }

    public override async Task<SlashResponse> Slash(
        SlashRequest request, ServerCallContext context)
    {
        var slashFactor = request.SlashFactor;
        await delegation.SlashAsync(slashFactor, context.CancellationToken);
        return new SlashResponse();
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
        var stakeInfo = await delegation.GetStakeInfoAsync(
            address, context.CancellationToken);
        return new GetStakeInfoResponse
        {
            StakeInfo = stakeInfo,
        };
    }
}
