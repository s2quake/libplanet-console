using Nekoyume.Action.Guild;

namespace LibplanetConsole.Console.Guild;

internal sealed class Guild(IConsole console)
    : ConsoleContentBase("guild"), IGuild
{
    public async Task CreateAsync(
        Address validatorAddress, CancellationToken cancellationToken)
    {
        var makeGuild = new MakeGuild(validatorAddress)
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

    public async Task BanMemberAsync(Address memberAddress, CancellationToken cancellationToken)
    {
        var banGuildMember = new BanGuildMember(new(memberAddress))
        {
        };
        await console.SendTransactionAsync([banGuildMember], cancellationToken);
    }

    public async Task UnbanMemberAsync(Address memberAddress, CancellationToken cancellationToken)
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

    public Task<GuildInfo> GetGuildAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    protected override async Task OnStopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
