using LibplanetConsole.Common;

namespace LibplanetConsole.Guild;

public sealed record class RequestJoinOptions : OptionsBase<RequestJoinOptions>
{
    public required AppAddress GuildAddress { get; init; }
}
