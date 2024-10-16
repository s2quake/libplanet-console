using LibplanetConsole.Common;

namespace LibplanetConsole.Bank.Services;

public sealed record class BurnOptions : OptionsBase<BurnOptions>
{
    public required decimal Amount { get; init; }
}
