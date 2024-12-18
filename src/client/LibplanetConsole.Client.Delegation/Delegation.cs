using Grpc.Core;
using Grpc.Net.Client;
using LibplanetConsole.Common;
using LibplanetConsole.Grpc.Delegation;
using Nekoyume.Action;
using static LibplanetConsole.Grpc.TypeUtility;

namespace LibplanetConsole.Client.Delegation;

internal sealed class Delegation(IClient client)
    : ClientContentBase("delegation"), IDelegation
{
    private GrpcChannel? _channel;
    private DelegationGrpcService.DelegationGrpcServiceClient? _service;

    public async Task StakeAsync(long ncg, CancellationToken cancellationToken)
    {
        var stake = new Stake(ncg);
        await client.SendTransactionAsync([stake], cancellationToken);
    }

    public async Task<DelegateeInfo> GetDelegateeInfoAsync(
        Address address, CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("The service is not initialized.");
        }

        var request = new GetDelegateeInfoRequest
        {
            Address = ToGrpc(address),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _service.GetDelegateeInfoAsync(request, callOptions);
        return response.DelegateeInfo;
    }

    public async Task<DelegatorInfo> GetDelegatorInfoAsync(
        Address address, CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("The service is not initialized.");
        }

        var request = new GetDelegatorInfoRequest
        {
            Address = ToGrpc(address),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _service.GetDelegatorInfoAsync(request, callOptions);
        return response.DelegatorInfo;
    }

    public async Task<StakeInfo> GetStakeInfoAsync(
        Address address, CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("The service is not initialized.");
        }

        var request = new GetStakeInfoRequest
        {
            Address = ToGrpc(address),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _service.GetStakeInfoAsync(request, callOptions);
        return response.StakeInfo;
    }

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        var nodeEndPoint = client.NodeEndPoint;
        var address = $"http://{EndPointUtility.ToString(nodeEndPoint)}";
        _channel = GrpcChannel.ForAddress(address);
        _service = new DelegationGrpcService.DelegationGrpcServiceClient(_channel);

        await Task.CompletedTask;
    }

    protected override async Task OnStopAsync(CancellationToken cancellationToken)
    {
        _channel?.Dispose();
        _channel = null;

        await Task.CompletedTask;
    }
}
