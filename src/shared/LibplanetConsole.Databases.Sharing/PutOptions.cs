using LibplanetConsole.Common;

namespace LibplanetConsole.Databases;

public sealed record class PutOptions : OptionsBase<PutOptions>
{
    public required string Key { get; init; } = string.Empty;

    public required string Value { get; init; } = string.Empty;
}
