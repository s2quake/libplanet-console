using LibplanetConsole.Common;

namespace LibplanetConsole.Guild;

public sealed record class JoinGuildOptions : OptionsBase<JoinGuildOptions>
{
    public required Address GuildAddress { get; init; }
}
