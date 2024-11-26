using LibplanetConsole.Common;
using LibplanetConsole.Console.Services;
using LibplanetConsole.Guild;

namespace LibplanetConsole.Console.Guild;

internal sealed class GuildNode : NodeContentBase, IGuild
{
    private readonly INode _node;

    public GuildNode(INode node)
        : base("guild")
    {
        _node = node;
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

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        // Info = await Service.GetGuildInfoAsync(default);
        throw new NotImplementedException();
    }

    protected override async Task OnStopAsync(CancellationToken cancellationToken)
    {
        // => Info = default;
        throw new NotImplementedException();
    }
}
