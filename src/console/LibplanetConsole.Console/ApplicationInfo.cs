using System.Text.Json.Serialization;
using LibplanetConsole.Common.Converters;

namespace LibplanetConsole.Console;

public readonly record struct ApplicationInfo
{
    [JsonConverter(typeof(EndPointJsonConverter))]
    public required EndPoint EndPoint { get; init; }

    public required string LogPath { get; init; }

    public bool NoProcess { get; init; }

    public bool Detach { get; init; }

    public bool NewWindow { get; init; }
}
