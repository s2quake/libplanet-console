using LibplanetConsole.Common;

namespace LibplanetConsole.Databases;

public sealed record class PutRangeOptions : OptionsBase<PutRangeOptions>
{
    public required KeyValue[] KeyValues { get; init; } = [];
}
