using LibplanetConsole.Common;

namespace LibplanetConsole.Client;

public readonly record struct ApplicationInfo
{
    public required EndPoint EndPoint { get; init; }

    public EndPoint? NodeEndPoint { get; init; }

    public required string LogPath { get; init; }
}
