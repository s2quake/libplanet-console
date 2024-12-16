using Grpc.Core;
using Grpc.Net.Client;
using LibplanetConsole.Common;
using LibplanetConsole.Grpc.Delegation;
using Microsoft.Extensions.DependencyInjection;
using static LibplanetConsole.Grpc.TypeUtility;

namespace LibplanetConsole.Console.Delegation;

internal sealed class ClientDelegation(
    [FromKeyedServices(IClient.Key)] IClient client)
    : ClientContentBase("client-delegation"), IClientDelegation
{
    private GrpcChannel? _channel;
    private DelegationGrpcService.DelegationGrpcServiceClient? _service;

    public async Task StakeAsync(long ncg, CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("Delegation service is not available.");
        }

        var request = new StakeRequest
        {
            Ncg = ncg,
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await _service.StakeAsync(request, callOptions);
    }

    public async Task<DelegateeInfo> GetDelegateeInfoAsync(
        Address address, CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("Delegation service is not available.");
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
            throw new InvalidOperationException("Delegation service is not available.");
        }

        var request = new GetDelegatorInfoRequest
        {
            Address = ToGrpc(address),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _service.GetDelegatorInfoAsync(request, callOptions);
        return response.DelegatorInfo;
    }

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        var address = $"http://{EndPointUtility.ToString(client.EndPoint)}";
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
