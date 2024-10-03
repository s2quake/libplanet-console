using System.Text.Json.Serialization;
using LibplanetConsole.Common.Converters;

namespace LibplanetConsole.Explorer;

public sealed record class ExplorerOptions
{
    [JsonConverter(typeof(EndPointJsonConverter))]
    public required EndPoint EndPoint { get; init; }
}
