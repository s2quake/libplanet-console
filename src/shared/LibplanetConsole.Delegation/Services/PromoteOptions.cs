using LibplanetConsole.Common;

namespace LibplanetConsole.Delegation.Services;

public sealed record class PromoteOptions : OptionsBase<PromoteOptions>
{
    public double Amount { get; init; }
}
