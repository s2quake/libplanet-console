using Grpc.Core;
using Grpc.Net.Client;
using LibplanetConsole.Common;
using LibplanetConsole.Grpc.Guild;
using Nekoyume.Action.Guild;
using static LibplanetConsole.Grpc.TypeUtility;

namespace LibplanetConsole.Client.Guild;

internal sealed class Guild(IClient client)
    : ClientContentBase(nameof(Guild)), IGuild
{
    public async Task CreateAsync(Address validatorAddress, CancellationToken cancellationToken)
    {
        var makeGuild = new MakeGuild(validatorAddress)
        {
        };
        await client.SendTransactionAsync([makeGuild], cancellationToken);
    }

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
        var removeGuild = new RemoveGuild
        {
        };
        await client.SendTransactionAsync([removeGuild], cancellationToken);
    }

    public async Task JoinAsync(Address guildAddress, CancellationToken cancellationToken)
    {
        var joinGuild = new JoinGuild(new(guildAddress))
        {
        };
        await client.SendTransactionAsync([joinGuild], cancellationToken);
    }

    public async Task LeaveAsync(CancellationToken cancellationToken)
    {
        var quitGuild = new QuitGuild
        {
        };
        await client.SendTransactionAsync([quitGuild], cancellationToken);
    }

    public async Task MoveAsync(Address guildAddress, CancellationToken cancellationToken)
    {
        var moveGuild = new MoveGuild(new(guildAddress))
        {
        };
        await client.SendTransactionAsync([moveGuild], cancellationToken);
    }

    public async Task BanAsync(Address memberAddress, CancellationToken cancellationToken)
    {
        var banGuildMember = new BanGuildMember(new(memberAddress))
        {
        };
        await client.SendTransactionAsync([banGuildMember], cancellationToken);
    }

    public async Task UnbanAsync(Address memberAddress, CancellationToken cancellationToken)
    {
        var unbanMemberGuild = new UnbanGuildMember(memberAddress)
        {
        };
        await client.SendTransactionAsync([unbanMemberGuild], cancellationToken);
    }

    public async Task ClaimAsync(CancellationToken cancellationToken)
    {
        var claimReward = new ClaimReward
        {
        };
        await client.SendTransactionAsync([claimReward], cancellationToken);
    }

    public async Task<GuildInfo> GetInfoAsync(
        Address memberAddress, CancellationToken cancellationToken)
    {
        var nodeEndPoint = client.NodeEndPoint;
        var address = $"http://{EndPointUtility.ToString(nodeEndPoint)}";
        using var channel = GrpcChannel.ForAddress(address);
        var service = new GuildGrpcService.GuildGrpcServiceClient(channel);
        var request = new GetInfoRequest
        {
            MemberAddress = ToGrpc(memberAddress),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await service.GetInfoAsync(request, callOptions);
        return response.GuildInfo;
    }
}
