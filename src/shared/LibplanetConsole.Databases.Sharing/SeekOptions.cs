using LibplanetConsole.Common;

namespace LibplanetConsole.Databases;

public sealed record class SeekOptions : OptionsBase<SeekOptions>
{
    public required string Key { get; init; } = string.Empty;
}
