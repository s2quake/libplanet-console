using LibplanetConsole.Common;

namespace LibplanetConsole.Guild;

public sealed record class AcceptJoinOptions : OptionsBase<AcceptJoinOptions>
{
    public required AppAddress MemberAddress { get; init; }
}
