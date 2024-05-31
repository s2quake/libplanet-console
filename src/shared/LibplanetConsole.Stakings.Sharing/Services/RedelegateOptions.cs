using Libplanet.Crypto;
using LibplanetConsole.Common;

namespace LibplanetConsole.Stakings.Services;

public sealed record class RedelegateOptions : OptionsBase<RedelegateOptions>
{
    public required Address SrcNodeAddress { get; init; }

    public required Address DestNodeAddress { get; init; }

    public long ShareAmount { get; init; }
}
