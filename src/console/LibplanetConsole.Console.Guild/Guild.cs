using Nekoyume.Action.Guild;

namespace LibplanetConsole.Console.Guild;

internal sealed class Guild(IConsole console)
    : ConsoleContentBase("guild"), IGuild
{
    public Task BanMemberAsync(Address memberAddress, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task ClaimAsync(CancellationToken cancellationToken)
    {
        var claimReward = new ClaimReward
        {
        };
        await console.SendTransactionAsync([claimReward], cancellationToken);
    }

    public async Task CreateAsync(
        Address validatorAddress, CancellationToken cancellationToken)
    {
        var makeGuild = new MakeGuild(validatorAddress)
        {
        };

        await console.SendTransactionAsync([makeGuild], cancellationToken);
    }

    public Task DeleteAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<GuildInfo> GetGuildAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task JoinAsync(Address guildAddress, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task LeaveAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task UnbanMemberAsync(Address memberAddress, CancellationToken cancellationToken)
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
