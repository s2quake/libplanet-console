using Grpc.Core;
using Grpc.Net.Client;
using LibplanetConsole.Common;
using LibplanetConsole.Grpc.Guild;
using Microsoft.Extensions.DependencyInjection;
using static LibplanetConsole.Grpc.TypeUtility;

namespace LibplanetConsole.Console.Guild;

internal sealed class NodeGuild(
    [FromKeyedServices(INode.Key)] INode node)
    : NodeContentBase("node-guild"), INodeGuild
{
    private GrpcChannel? _channel;
    private GuildGrpcService.GuildGrpcServiceClient? _service;

    public async Task CreateAsync(Address validatorAddress, CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("The service is not initialized.");
        }

        var request = new CreateRequest
        {
            ValidatorAddress = ToGrpc(validatorAddress),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await _service.CreateAsync(request, callOptions);
    }

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("The service is not initialized.");
        }

        var request = new DeleteRequest();
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await _service.DeleteAsync(request, callOptions);
    }

    public async Task JoinAsync(Address guildAddress, CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("The service is not initialized.");
        }

        var request = new JoinRequest
        {
            GuildAddress = ToGrpc(guildAddress),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await _service.JoinAsync(request, callOptions);
    }

    public async Task LeaveAsync(CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("The service is not initialized.");
        }

        var request = new LeaveRequest();
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await _service.LeaveAsync(request, callOptions);
    }

    public async Task MoveAsync(Address guildAddress, CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("The service is not initialized.");
        }

        var request = new MoveRequest
        {
            GuildAddress = ToGrpc(guildAddress),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await _service.MoveAsync(request, callOptions);
    }

    public async Task BanAsync(Address memberAddress, CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("The service is not initialized.");
        }

        var request = new BanRequest
        {
            MemberAddress = ToGrpc(memberAddress),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await _service.BanAsync(request, callOptions);
    }

    public async Task UnbanAsync(Address memberAddress, CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("The service is not initialized.");
        }

        var request = new UnbanRequest
        {
            MemberAddress = ToGrpc(memberAddress),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await _service.UnbanAsync(request, callOptions);
    }

    public async Task ClaimAsync(CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("The service is not initialized.");
        }

        var request = new ClaimRequest();
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await _service.ClaimAsync(request, callOptions);
    }

    public async Task<GuildInfo> GetInfoAsync(
        Address memberAddress, CancellationToken cancellationToken)
    {
        if (_service is null)
        {
            throw new InvalidOperationException("The service is not initialized.");
        }

        var request = new GetInfoRequest
        {
            MemberAddress = ToGrpc(memberAddress),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await _service.GetInfoAsync(request, callOptions);
        return response.GuildInfo;
    }

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        var address = $"http://{EndPointUtility.ToString(node.EndPoint)}";
        _channel = GrpcChannel.ForAddress(address);
        _service = new GuildGrpcService.GuildGrpcServiceClient(_channel);

        await Task.CompletedTask;
    }

    protected override async Task OnStopAsync(CancellationToken cancellationToken)
    {
        _channel?.Dispose();
        _channel = null;

        await Task.CompletedTask;
    }
}
