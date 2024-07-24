using LibplanetConsole.Common;

namespace LibplanetConsole.Guild;

public sealed record class UnbanMemberOptions : OptionsBase<UnbanMemberOptions>
{
    public required AppAddress MemberAddress { get; init; }
}
