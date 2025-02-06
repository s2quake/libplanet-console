using Grpc.Core;
using LibplanetConsole.Console.Services;
using LibplanetConsole.Delegation;
using LibplanetConsole.Delegation.Grpc;
using static LibplanetConsole.Grpc.TypeUtility;
using TService = LibplanetConsole.Delegation.Grpc.DelegationGrpcService.DelegationGrpcServiceClient;

namespace LibplanetConsole.Console.Delegation;

internal sealed class ClientDelegation(
    [FromKeyedServices(IClient.Key)] IClient client)
    : GrpcClientContentBase<TService>(client, "client-delegation"), IClientDelegation
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
