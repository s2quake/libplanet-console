using Libplanet.Crypto;
using LibplanetConsole.Common;

namespace LibplanetConsole.Stakings.Services;

public sealed record class DelegateOptions : OptionsBase<DelegateOptions>
{
    public required Address NodeAddress { get; init; }

    public double Amount { get; init; }
}
