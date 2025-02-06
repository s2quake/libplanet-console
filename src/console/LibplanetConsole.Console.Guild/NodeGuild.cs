using Grpc.Core;
using LibplanetConsole.Console.Services;
using LibplanetConsole.Guild;
using LibplanetConsole.Guild.Grpc;
using static LibplanetConsole.Grpc.TypeUtility;
using TService = LibplanetConsole.Guild.Grpc.GuildGrpcService.GuildGrpcServiceClient;

namespace LibplanetConsole.Console.Guild;

internal sealed class NodeGuild(
    [FromKeyedServices(INode.Key)] INode node)
    : GrpcNodeContentBase<TService>(node, "node-guild"), INodeGuild
{
    public async Task CreateAsync(Address nodeAddress, CancellationToken cancellationToken)
    {
        var request = new CreateRequest
        {
            ValidatorAddress = ToGrpc(nodeAddress),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await Service.CreateAsync(request, callOptions);
    }

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
        var request = new DeleteRequest();
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await Service.DeleteAsync(request, callOptions);
    }

    public async Task JoinAsync(Address guildAddress, CancellationToken cancellationToken)
    {
        var request = new JoinRequest
        {
            GuildAddress = ToGrpc(guildAddress),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await Service.JoinAsync(request, callOptions);
    }

    public async Task LeaveAsync(CancellationToken cancellationToken)
    {
        var request = new LeaveRequest();
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await Service.LeaveAsync(request, callOptions);
    }

    public async Task MoveAsync(Address guildAddress, CancellationToken cancellationToken)
    {
        var request = new MoveRequest
        {
            GuildAddress = ToGrpc(guildAddress),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await Service.MoveAsync(request, callOptions);
    }

    public async Task BanAsync(Address memberAddress, CancellationToken cancellationToken)
    {
        var request = new BanRequest
        {
            MemberAddress = ToGrpc(memberAddress),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await Service.BanAsync(request, callOptions);
    }

    public async Task UnbanAsync(Address memberAddress, CancellationToken cancellationToken)
    {
        var request = new UnbanRequest
        {
            MemberAddress = ToGrpc(memberAddress),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await Service.UnbanAsync(request, callOptions);
    }

    public async Task ClaimAsync(CancellationToken cancellationToken)
    {
        var request = new ClaimRequest();
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await Service.ClaimAsync(request, callOptions);
    }

    public async Task<GuildInfo> GetInfoAsync(
        Address memberAddress, CancellationToken cancellationToken)
    {
        var request = new GetInfoRequest
        {
            MemberAddress = ToGrpc(memberAddress),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await Service.GetInfoAsync(request, callOptions);
        return response.GuildInfo;
    }
}
