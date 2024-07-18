using System.ComponentModel.Composition;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Consoles.Services;
using LibplanetConsole.Guild;
using LibplanetConsole.Guild.Services;

namespace LibplanetConsole.Consoles.Guild;

[Export(typeof(IGuildClientContent))]
[Export(typeof(IClientContentService))]
[Export(typeof(IClientContent))]
[method: ImportingConstructor]
internal sealed class GuildClientContent(IClient client)
        : ClientContentBase(client), IGuildClientContent, IClientContentService
{
    private readonly RemoteService<IGuildClientService> _remoteService = new();

    IRemoteService IClientContentService.RemoteService => _remoteService;

    private IGuildClientService Service => _remoteService.Service;

    public Task BanMemberAsync(BanMemberOptions options, CancellationToken cancellationToken)
        => Service.BanMemberAsync(options, cancellationToken);

    public Task CreateAsync(CreateGuildOptions options, CancellationToken cancellationToken)
        => Service.CreateAsync(options, cancellationToken);

    public Task DeleteAsync(DeleteGuildOptions options, CancellationToken cancellationToken)
        => Service.DeleteAsync(options, cancellationToken);

    public Task JoinAsync(JoinGuildOptions options, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task QuitAsync(LeaveGuildOptions options, CancellationToken cancellationToken)
        => Service.QuitAsync(options, cancellationToken);

    public Task UnbanMemberAsync(UnbanMemberOptions options, CancellationToken cancellationToken)
        => Service.UnbanMemberAsync(options, cancellationToken);
}
