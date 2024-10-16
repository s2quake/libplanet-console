using System.ComponentModel.Composition;
using LibplanetConsole.Common;
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
        => guildNode.CreateAsync(options.Verify(node), cancellationToken);

    public Task<AppAddress> DeleteAsync(
        DeleteGuildOptions options, CancellationToken cancellationToken)
        => guildNode.DeleteAsync(options.Verify(node), cancellationToken);

    public Task QuitAsync(LeaveGuildOptions options, CancellationToken cancellationToken)
        => guildNode.QuitAsync(options.Verify(node), cancellationToken);

    public Task RequestJoinAsync(RequestJoinOptions options, CancellationToken cancellationToken)
        => guildNode.RequestJoinAsync(options.Verify(node), cancellationToken);

    public Task CancelJoinAsync(CancelJoinOptions options, CancellationToken cancellationToken)
        => guildNode.CancelJoinAsync(options.Verify(node), cancellationToken);

    public Task AcceptJoinAsync(AcceptJoinOptions options, CancellationToken cancellationToken)
        => guildNode.AcceptJoinAsync(options.Verify(node), cancellationToken);

    public Task RejectJoinAsync(RejectJoinOptions options, CancellationToken cancellationToken)
        => guildNode.RejectJoinAsync(options.Verify(node), cancellationToken);

    public Task BanMemberAsync(BanMemberOptions options, CancellationToken cancellationToken)
        => guildNode.BanMemberAsync(options.Verify(node), cancellationToken);

    public Task UnbanMemberAsync(UnbanMemberOptions options, CancellationToken cancellationToken)
        => guildNode.UnbanMemberAsync(options.Verify(node), cancellationToken);

    public Task<GuildInfo> GetGuildInfoAsync(CancellationToken cancellationToken)
        => Task.Run(() => guildNode.Info, cancellationToken);

    public Task<AppAddress> GetGuildAsync(
        long height, AppAddress address, CancellationToken cancellationToken)
        => guildNode.GetGuildAsync(height, address, cancellationToken);

    public Task<AppAddress[]> GetGuildMembersAsync(
        long height, AppAddress guildAddress, CancellationToken cancellationToken)
        => guildNode.GetGuildMembersAsync(height, guildAddress, cancellationToken);
}
