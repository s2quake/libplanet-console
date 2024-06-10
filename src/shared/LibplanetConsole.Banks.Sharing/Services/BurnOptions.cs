using LibplanetConsole.Common;

namespace LibplanetConsole.Banks.Services;

public sealed record class BurnOptions : OptionsBase<BurnOptions>
{
    public required double Amount { get; init; }
}
