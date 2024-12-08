namespace LibplanetConsole.Console.Guild;

internal sealed class GuildClient : ClientContentBase, IGuild
{
    public GuildClient()
        : base("guild-client")
    {
    }

    public GuildInfo Info { get; private set; }

    public Task BanMemberAsync(Address memberAddress, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task CreateAsync(string name, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Address> DeleteAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Address> GetGuildAsync(long height, Address address, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Address[]> GetGuildMembersAsync(long height, Address guildAddress, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task QuitAsync(CancellationToken cancellationToken)
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
