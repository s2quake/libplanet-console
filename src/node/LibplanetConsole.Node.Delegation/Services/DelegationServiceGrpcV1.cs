using System.Globalization;
using System.Numerics;
using Grpc.Core;
using LibplanetConsole.Grpc.Delegation;
using LibplanetConsole.Node.Bank;

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
        var share = BigInteger.Parse(request.Share, NumberStyles.AllowDecimalPoint);
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

    public override async Task<GetInfoResponse> GetInfo(
        GetInfoRequest request, ServerCallContext context)
    {
        var info = await delegation.GetInfoAsync(context.CancellationToken);
        return new GetInfoResponse { DelegationInfo = info };
    }

    public override Task<GetStakeInfoResponse> GetStakeInfo(
        GetStakeInfoRequest request, ServerCallContext context)
    {
        return base.GetStakeInfo(request, context);
    }
}
