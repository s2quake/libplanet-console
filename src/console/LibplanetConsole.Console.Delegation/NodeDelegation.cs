using System.Numerics;
using Grpc.Core;
using LibplanetConsole.Bank;
using LibplanetConsole.Console.Services;
using LibplanetConsole.Delegation;
using LibplanetConsole.Delegation.Grpc;
using static LibplanetConsole.Grpc.TypeUtility;
using TService = LibplanetConsole.Delegation.Grpc.DelegationGrpcService.DelegationGrpcServiceClient;

namespace LibplanetConsole.Console.Delegation;

internal sealed class NodeDelegation(
    [FromKeyedServices(INode.Key)] INode node,
    ICurrencyCollection currencies)
    : GrpcNodeContentBase<TService>(node, "node-delegation"), INodeDelegation
{
    public async Task StakeAsync(long ncg, CancellationToken cancellationToken)
    {
        var request = new StakeRequest
        {
            Ncg = ncg,
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await Service.StakeAsync(request, callOptions);
    }

    public async Task PromoteAsync(
        FungibleAssetValue guildGold, CancellationToken cancellationToken)
    {
        var request = new PromoteRequest
        {
            GuildGold = currencies.ToString(guildGold),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await Service.PromoteAsync(request, callOptions);
    }

    public async Task DelegateAsync(
        FungibleAssetValue guildGold, CancellationToken cancellationToken)
    {
        var request = new DelegateRequest
        {
            GuildGold = currencies.ToString(guildGold),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await Service.DelegateAsync(request, callOptions);
    }

    public async Task UndelegateAsync(BigInteger share, CancellationToken cancellationToken)
    {
        var request = new UndelegateRequest
        {
            Share = BigIntegerUtility.ToString(share),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await Service.UndelegateAsync(request, callOptions);
    }

    public async Task UnjailAsync(CancellationToken cancellationToken)
    {
        var request = new UnjailRequest();
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await Service.UnjailAsync(request, callOptions);
    }

    public async Task SetCommissionAsync(long commission, CancellationToken cancellationToken)
    {
        var request = new SetCommissionRequest
        {
            Commission = commission,
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await Service.SetCommissionAsync(request, callOptions);
    }

    public async Task ClaimAsync(CancellationToken cancellationToken)
    {
        var request = new ClaimRequest();
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await Service.ClaimAsync(request, callOptions);
    }

    public async Task SlashAsync(long slashFactor, CancellationToken cancellationToken)
    {
        var request = new SlashRequest
        {
            SlashFactor = slashFactor,
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await Service.SlashAsync(request, callOptions);
    }

    public async Task<DelegateeInfo> GetDelegateeInfoAsync(
        Address address, CancellationToken cancellationToken)
    {
        var request = new GetDelegateeInfoRequest
        {
            Address = ToGrpc(address),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await Service.GetDelegateeInfoAsync(request, callOptions);
        return response.DelegateeInfo;
    }

    public async Task<DelegatorInfo> GetDelegatorInfoAsync(
        Address address, CancellationToken cancellationToken)
    {
        var request = new GetDelegatorInfoRequest
        {
            Address = ToGrpc(address),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await Service.GetDelegatorInfoAsync(request, callOptions);
        return response.DelegatorInfo;
    }

    public async Task<StakeInfo> GetStakeInfoAsync(
        Address address, CancellationToken cancellationToken)
    {
        var request = new GetStakeInfoRequest
        {
            Address = ToGrpc(address),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await Service.GetStakeInfoAsync(request, callOptions);
        return response.StakeInfo;
    }
}
