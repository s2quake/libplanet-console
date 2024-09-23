using LibplanetConsole.Common;

namespace LibplanetConsole.Node;

public readonly record struct ApplicationInfo
{
    public required AppEndPoint EndPoint { get; init; }

    public required AppEndPoint? SeedEndPoint { get; init; }

    public required string StorePath { get; init; }

    public required string LogPath { get; init; }

    public int ParentProcessId { get; init; }
}
