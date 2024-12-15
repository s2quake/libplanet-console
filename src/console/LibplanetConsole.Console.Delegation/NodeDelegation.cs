using System.Numerics;
using Grpc.Core;
using Grpc.Net.Client;
using LibplanetConsole.Common;
using LibplanetConsole.Console.Bank;
using LibplanetConsole.Grpc.Delegation;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Delegation;

internal sealed class NodeDelegation(
    [FromKeyedServices(INode.Key)] INode node,
    [FromKeyedServices(INode.Key)] INodeBank bank,
    ICurrencyCollection currencies)
    : NodeContentBase("node-delegation"), INodeDelegation
{
    private GrpcChannel? _channel;
    private DelegationGrpcService.DelegationGrpcServiceClient? _service;

    public async Task StakeAsync(long ncg, CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("Validator service is not available.");
        }

        var request = new StakeRequest
        {
            Ncg = ncg,
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await _service.StakeAsync(request, callOptions);
    }

    public async Task PromoteAsync(
        FungibleAssetValue guildGold, CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("Validator service is not available.");
        }

        var request = new PromoteRequest
        {
            GuildGold = currencies.ToString(guildGold),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await _service.PromoteAsync(request, callOptions);
    }

    public async Task DelegateAsync(
        FungibleAssetValue guildGold, CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("Validator service is not available.");
        }

        var request = new DelegateRequest
        {
            GuildGold = currencies.ToString(guildGold),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await _service.DelegateAsync(request, callOptions);
    }

    public async Task UndelegateAsync(BigInteger share, CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("Validator service is not available.");
        }

        var request = new UndelegateRequest
        {
            Share = BigIntegerUtility.ToString(share),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await _service.UndelegateAsync(request, callOptions);
    }

    public async Task UnjailAsync(CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("Validator service is not available.");
        }

        var request = new UnjailRequest();
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await _service.UnjailAsync(request, callOptions);
    }

    public async Task SetCommissionAsync(long commission, CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("Validator service is not available.");
        }

        var request = new SetCommissionRequest
        {
            Commission = commission,
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await _service.SetCommissionAsync(request, callOptions);
    }

    public async Task ClaimAsync(CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("Validator service is not available.");
        }

        var request = new ClaimRequest();
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await _service.ClaimAsync(request, callOptions);
    }

    public async Task<DelegationInfo> GetInfoAsync(CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("Validator service is not available.");
        }

        var request = new GetInfoRequest();
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _service.GetInfoAsync(request, callOptions);
        await _service.GetInfoAsync(request, callOptions);
        return response.DelegationInfo;
    }

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        var address = $"http://{EndPointUtility.ToString(node.EndPoint)}";
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
