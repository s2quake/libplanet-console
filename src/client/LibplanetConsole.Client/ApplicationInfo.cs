using System.Text.Json.Serialization;
using LibplanetConsole.Common.Converters;

namespace LibplanetConsole.Client;

public readonly record struct ApplicationInfo
{
    [JsonConverter(typeof(EndPointJsonConverter))]
    public required EndPoint EndPoint { get; init; }

    [JsonConverter(typeof(EndPointJsonConverter))]
    public EndPoint? NodeEndPoint { get; init; }

    public required string LogPath { get; init; }
}
