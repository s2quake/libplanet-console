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

    public Task AcceptJoinAsync(AcceptJoinOptions options, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task BanMemberAsync(BanMemberOptions options, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task CancelJoinAsync(CancelJoinOptions options, CancellationToken cancellationToken)
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

    public Task RejectJoinAsync(RejectJoinOptions options, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task RequestJoinAsync(RequestJoinOptions options, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task UnbanMemberAsync(UnbanMemberOptions options, CancellationToken cancellationToken)
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
