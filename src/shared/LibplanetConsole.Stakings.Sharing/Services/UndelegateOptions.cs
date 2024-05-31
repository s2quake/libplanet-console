using Libplanet.Crypto;
using LibplanetConsole.Common;

namespace LibplanetConsole.Stakings.Services;

public sealed record class UndelegateOptions : OptionsBase<UndelegateOptions>
{
    public required Address NodeAddress { get; init; }

    public long ShareAmount { get; init; }
}
