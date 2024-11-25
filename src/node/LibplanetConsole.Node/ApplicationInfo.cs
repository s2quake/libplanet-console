using System.Text.Json.Serialization;
using LibplanetConsole.Common.Converters;

namespace LibplanetConsole.Node;

public readonly record struct ApplicationInfo
{
    [JsonConverter(typeof(EndPointJsonConverter))]
    public required EndPoint? SeedEndPoint { get; init; }

    public required string StorePath { get; init; }

    public required string LogPath { get; init; }

    public int ParentProcessId { get; init; }

    public bool IsSingleNode { get; init; }
}
