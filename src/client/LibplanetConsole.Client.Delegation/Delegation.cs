using Grpc.Core;
using LibplanetConsole.Client.Services;
using LibplanetConsole.Delegation;
using LibplanetConsole.Delegation.Grpc;
using Nekoyume.Action;
using static LibplanetConsole.Grpc.TypeUtility;
using TService = LibplanetConsole.Delegation.Grpc.DelegationGrpcService.DelegationGrpcServiceClient;

namespace LibplanetConsole.Client.Delegation;

internal sealed class Delegation(IClient client)
    : GrpcClientContentBase<TService>(client, "delegation"),
    IDelegation
{
    private readonly IClient _client = client;

    public async Task StakeAsync(long ncg, CancellationToken cancellationToken)
    {
        var stake = new Stake(ncg);
        await _client.SendTransactionAsync([stake], cancellationToken);
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
