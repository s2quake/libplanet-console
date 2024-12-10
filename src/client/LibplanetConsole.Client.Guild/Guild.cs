namespace LibplanetConsole.Client.Guild;

internal sealed class Guild(IClient client, IBlockChain blockChain)
    : ClientContentBase(nameof(Guild)), IGuild
{
    public Task BanMemberAsync(Address memberAddress, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<GuildInfo> CreateAsync(Address validatorAddress, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Address> DeleteAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<GuildInfo> GetGuildAsync(Address address, CancellationToken cancellationToken)
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

    protected override Task OnStartAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override Task OnStopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
