using LibplanetConsole.Common;

namespace LibplanetConsole.Guild;

public sealed record class UnbanMemberOptions : OptionsBase<UnbanMemberOptions>
{
    public required Address MemberAddress { get; init; }
}
