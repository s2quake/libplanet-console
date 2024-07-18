using System.ComponentModel.Composition;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Guild;
using LibplanetConsole.Guild.Services;

namespace LibplanetConsole.Nodes.Guild.Services;

[Export(typeof(ILocalService))]
[method: ImportingConstructor]
internal sealed class GuildNodeService(INode node, GuildNode guildNode)
        : LocalService<IGuildNodeService>, IGuildNodeService
{
    public Task CreateAsync(CreateGuildOptions options, CancellationToken cancellationToken)
    {
        options.Verify(node);
        return guildNode.CreateAsync(options, cancellationToken);
    }

    public Task DeleteAsync(DeleteGuildOptions options, CancellationToken cancellationToken)
    {
        options.Verify(node);
        return guildNode.DeleteAsync(options, cancellationToken);
    }

    public Task JoinAsync(JoinGuildOptions options, CancellationToken cancellationToken)
    {
        options.Verify(node);
        return guildNode.JoinAsync(options, cancellationToken);
    }

    public Task QuitAsync(LeaveGuildOptions options, CancellationToken cancellationToken)
    {
        options.Verify(node);
        return guildNode.QuitAsync(options, cancellationToken);
    }

    public Task BanMemberAsync(BanMemberOptions options, CancellationToken cancellationToken)
    {
        options.Verify(node);
        return guildNode.BanMemberAsync(options, cancellationToken);
    }

    public Task UnbanMemberAsync(UnbanMemberOptions options, CancellationToken cancellationToken)
    {
        options.Verify(node);
        return guildNode.UnbanMemberAsync(options, cancellationToken);
    }

    public Task<GuildInfo> StartAsync(CancellationToken cancellationToken)
    {
        return guildNode.StartAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return guildNode.StopAsync(cancellationToken);
    }

    public Task<GuildInfo> GetGuildInfoAsync(CancellationToken cancellationToken)
    {
        return Task.Run(() => guildNode.Info, cancellationToken);
    }
}
