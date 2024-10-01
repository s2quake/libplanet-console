using LibplanetConsole.Common;

namespace LibplanetConsole.Bank.Services;

public sealed record class MintOptions : OptionsBase<MintOptions>
{
    public required decimal Amount { get; init; }
}
