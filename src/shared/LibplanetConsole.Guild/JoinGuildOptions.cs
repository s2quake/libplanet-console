using LibplanetConsole.Common;

namespace LibplanetConsole.Guild;

public sealed record class JoinGuildOptions : OptionsBase<JoinGuildOptions>
{
    public required AppAddress GuildAddress { get; init; }
}
