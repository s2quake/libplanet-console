using LibplanetConsole.Common;

namespace LibplanetConsole.Banks.Services;

public sealed record class MintOptions : OptionsBase<MintOptions>
{
    public required double Amount { get; init; }
}
