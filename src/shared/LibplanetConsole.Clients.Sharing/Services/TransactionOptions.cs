using LibplanetConsole.Common;

namespace LibplanetConsole.Clients.Services;

public sealed record class TransactionOptions : OptionsBase<TransactionOptions>
{
    public required string Text { get; init; } = string.Empty;
}
