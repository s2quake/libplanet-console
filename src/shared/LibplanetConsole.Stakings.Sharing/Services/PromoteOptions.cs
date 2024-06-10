using LibplanetConsole.Common;

namespace LibplanetConsole.Stakings.Services;

public sealed record class PromoteOptions : OptionsBase<PromoteOptions>
{
    public double Amount { get; init; }
}
