using LibplanetConsole.Guild;
using Nekoyume.Action.Guild;

namespace LibplanetConsole.Console.Guild;

internal sealed class Guild(IConsole console, RunningNode runningNode)
    : ConsoleContentBase("guild"), IGuild
{
    public async Task CreateAsync(
        Address nodeAddress, CancellationToken cancellationToken)
    {
        var makeGuild = new MakeGuild(nodeAddress)
        {
        };
        await console.SendTransactionAsync([makeGuild], cancellationToken);
    }

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
        var removeGuild = new RemoveGuild
        {
        };
        await console.SendTransactionAsync([removeGuild], cancellationToken);
    }

    public async Task JoinAsync(Address guildAddress, CancellationToken cancellationToken)
    {
        var joinGuild = new JoinGuild(new(guildAddress))
        {
        };
        await console.SendTransactionAsync([joinGuild], cancellationToken);
    }

    public async Task LeaveAsync(CancellationToken cancellationToken)
    {
        var quitGuild = new QuitGuild
        {
        };
        await console.SendTransactionAsync([quitGuild], cancellationToken);
    }

    public async Task MoveAsync(Address guildAddress, CancellationToken cancellationToken)
    {
        var moveGuild = new MoveGuild(new(guildAddress))
        {
        };
        await console.SendTransactionAsync([moveGuild], cancellationToken);
    }

    public async Task BanAsync(Address memberAddress, CancellationToken cancellationToken)
    {
        var banGuildMember = new BanGuildMember(new(memberAddress))
        {
        };
        await console.SendTransactionAsync([banGuildMember], cancellationToken);
    }

    public async Task UnbanAsync(Address memberAddress, CancellationToken cancellationToken)
    {
        var unbanMemberGuild = new UnbanGuildMember(memberAddress)
        {
        };
        await console.SendTransactionAsync([unbanMemberGuild], cancellationToken);
    }

    public async Task ClaimAsync(CancellationToken cancellationToken)
    {
        var claimReward = new ClaimReward
        {
        };
        await console.SendTransactionAsync([claimReward], cancellationToken);
    }

    public Task<GuildInfo> GetInfoAsync(Address memberAddress, CancellationToken cancellationToken)
    {
        var guildNode = runningNode.Node.GetRequiredKeyedService<INodeGuild>(INode.Key);
        return guildNode.GetInfoAsync(memberAddress, cancellationToken);
    }
}
