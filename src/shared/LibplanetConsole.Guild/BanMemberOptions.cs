using LibplanetConsole.Common;

namespace LibplanetConsole.Guild;

public sealed record class BanMemberOptions : OptionsBase<BanMemberOptions>
{
    public required Address MemberAddress { get; init; }
}
