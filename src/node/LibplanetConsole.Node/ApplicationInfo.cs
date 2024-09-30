namespace LibplanetConsole.Node;

public readonly record struct ApplicationInfo
{
    public required EndPoint EndPoint { get; init; }

    public required EndPoint? SeedEndPoint { get; init; }

    public required string StorePath { get; init; }

    public required string LogPath { get; init; }

    public int ParentProcessId { get; init; }
}
