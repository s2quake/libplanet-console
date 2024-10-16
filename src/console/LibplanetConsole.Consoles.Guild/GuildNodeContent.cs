using System.ComponentModel.Composition;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Consoles.Services;
using LibplanetConsole.Guild;
using LibplanetConsole.Guild.Services;

namespace LibplanetConsole.Consoles.Guild;

[Export(typeof(IGuildNodeContent))]
[Export(typeof(INodeContentService))]
[Export(typeof(INodeContent))]
internal sealed class GuildNodeContent
    : NodeContentBase, IGuildNodeContent, INodeContentService, IDisposable
{
    private readonly RemoteService<IGuildNodeService> _remoteService = new();
    private readonly INode _node;

    [ImportingConstructor]
    public GuildNodeContent(INode node)
        : base(node)
    {
        _node = node;
        _node.Started += Node_Started;
        _node.Stopped += Node_Stopped;
    }

    public GuildInfo Info { get; private set; }

    IRemoteService INodeContentService.RemoteService => _remoteService;

    private IGuildNodeService Service => _remoteService.Service;

    public Task BanMemberAsync(BanMemberOptions options, CancellationToken cancellationToken)
        => Service.BanMemberAsync(options, cancellationToken);

    public Task CreateAsync(CreateGuildOptions options, CancellationToken cancellationToken)
        => Service.CreateAsync(options, cancellationToken);

    public Task<AppAddress> DeleteAsync(
        DeleteGuildOptions options, CancellationToken cancellationToken)
        => Service.DeleteAsync(options, cancellationToken);

    public Task RequestJoinAsync(RequestJoinOptions options, CancellationToken cancellationToken)
        => Service.RequestJoinAsync(options, cancellationToken);

    public Task CancelJoinAsync(CancelJoinOptions options, CancellationToken cancellationToken)
        => Service.CancelJoinAsync(options, cancellationToken);

    public Task AcceptJoinAsync(AcceptJoinOptions options, CancellationToken cancellationToken)
        => Service.AcceptJoinAsync(options, cancellationToken);

    public Task RejectJoinAsync(RejectJoinOptions options, CancellationToken cancellationToken)
        => Service.RejectJoinAsync(options, cancellationToken);

    public Task QuitAsync(LeaveGuildOptions options, CancellationToken cancellationToken)
        => Service.QuitAsync(options, cancellationToken);

    public Task UnbanMemberAsync(UnbanMemberOptions options, CancellationToken cancellationToken)
        => Service.UnbanMemberAsync(options, cancellationToken);

    public Task<AppAddress> GetGuildAsync(
        long height, AppAddress address, CancellationToken cancellationToken)
        => Service.GetGuildAsync(height, address, cancellationToken);

    public Task<AppAddress[]> GetGuildMembersAsync(
        long height, AppAddress guildAddress, CancellationToken cancellationToken)
        => Service.GetGuildMembersAsync(height, guildAddress, cancellationToken);

    void IDisposable.Dispose()
    {
        _node.Started -= Node_Started;
        _node.Stopped -= Node_Stopped;
    }

    private async void Node_Started(object? sender, EventArgs e)
        => Info = await Service.GetGuildInfoAsync(default);

    private void Node_Stopped(object? sender, EventArgs e)
        => Info = default;
}
