using LibplanetConsole.Common;
using LibplanetConsole.Console.Services;
using LibplanetConsole.Guild;

namespace LibplanetConsole.Console.Guild;

internal sealed class GuildClient : ClientContentBase, IGuild
{
    public GuildClient()
        : base("guild-client")
    {
    }

    public GuildInfo Info { get; private set; }

    public Task BanMemberAsync(BanMemberOptions options, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task CreateAsync(CreateGuildOptions options, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Address> DeleteAsync(DeleteGuildOptions options, CancellationToken cancellationToken)
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

    public Task QuitAsync(LeaveGuildOptions options, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task UnbanMemberAsync(UnbanMemberOptions options, CancellationToken cancellationToken)
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
