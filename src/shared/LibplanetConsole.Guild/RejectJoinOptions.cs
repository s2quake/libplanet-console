using LibplanetConsole.Common;

namespace LibplanetConsole.Guild;

public sealed record class RejectJoinOptions : OptionsBase<RejectJoinOptions>
{
    public required AppAddress MemberAddress { get; init; }
}
