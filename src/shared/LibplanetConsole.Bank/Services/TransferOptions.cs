using LibplanetConsole.Common;

namespace LibplanetConsole.Bank.Services;

public sealed record class TransferOptions : OptionsBase<TransferOptions>
{
    public required Address TargetAddress { get; init; }

    public required decimal Amount { get; init; }
}
