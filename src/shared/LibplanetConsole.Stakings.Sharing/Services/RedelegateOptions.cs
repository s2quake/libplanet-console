using LibplanetConsole.Common;

namespace LibplanetConsole.Stakings.Services;

public sealed record class RedelegateOptions : OptionsBase<RedelegateOptions>
{
    public required AppAddress SrcNodeAddress { get; init; }

    public required AppAddress DestNodeAddress { get; init; }

    public long ShareAmount { get; init; }
}
