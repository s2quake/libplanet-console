using LibplanetConsole.Common;

namespace LibplanetConsole.Banks.Services;

public sealed record class TransferOptions : OptionsBase<TransferOptions>
{
    public required AppAddress TargetAddress { get; init; }

    public required double Amount { get; init; }
}
