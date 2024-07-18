using LibplanetConsole.Guild;

namespace LibplanetConsole.Consoles.Guild;

public interface IGuildNodeContent
{
    Task CreateAsync(CreateGuildOptions options, CancellationToken cancellationToken);

    Task DeleteAsync(DeleteGuildOptions options, CancellationToken cancellationToken);

    Task JoinAsync(JoinGuildOptions options, CancellationToken cancellationToken);

    Task QuitAsync(LeaveGuildOptions options, CancellationToken cancellationToken);

    Task BanMemberAsync(BanMemberOptions options, CancellationToken cancellationToken);

    Task UnbanMemberAsync(UnbanMemberOptions options, CancellationToken cancellationToken);
}
