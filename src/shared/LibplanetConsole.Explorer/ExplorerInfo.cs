using System.Text.Json.Serialization;
using LibplanetConsole.Common.Converters;

namespace LibplanetConsole.Explorer;

public readonly record struct ExplorerInfo
{
    [JsonConverter(typeof(EndPointJsonConverter))]
    public EndPoint? EndPoint { get; init; }

    public bool IsRunning { get; init; }

    public string Url { get; init; }
}
