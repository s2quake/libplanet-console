using System.ComponentModel.Composition;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Consoles.Services;
using LibplanetConsole.Guild;
using LibplanetConsole.Guild.Services;

namespace LibplanetConsole.Consoles.Guild;

[Export(typeof(IGuildNodeContent))]
[Export(typeof(INodeContentService))]
[Export(typeof(INodeContent))]
[method: ImportingConstructor]
internal sealed class GuildNodeContent(INode node)
        : NodeContentBase(node), IGuildNodeContent, INodeContentService
{
    private readonly RemoteService<IGuildNodeService> _remoteService = new();

    IRemoteService INodeContentService.RemoteService => _remoteService;

    private IGuildNodeService Service => _remoteService.Service;

    public Task BanMemberAsync(BanMemberOptions options, CancellationToken cancellationToken)
        => Service.BanMemberAsync(options, cancellationToken);

    public Task CreateAsync(CreateGuildOptions options, CancellationToken cancellationToken)
        => Service.CreateAsync(options, cancellationToken);

    public Task DeleteAsync(DeleteGuildOptions options, CancellationToken cancellationToken)
        => Service.DeleteAsync(options, cancellationToken);

    public Task JoinAsync(JoinGuildOptions options, CancellationToken cancellationToken)
        => Service.JoinAsync(options, cancellationToken);

    public Task QuitAsync(LeaveGuildOptions options, CancellationToken cancellationToken)
        => Service.QuitAsync(options, cancellationToken);

    public Task UnbanMemberAsync(UnbanMemberOptions options, CancellationToken cancellationToken)
        => Service.UnbanMemberAsync(options, cancellationToken);
}
