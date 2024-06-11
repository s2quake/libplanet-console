namespace LibplanetConsole.Nodes.Serializations;

public readonly record struct ApplicationInfo
{
    public required string EndPoint { get; init; }

    public required string NodeEndPoint { get; init; }

    public required string StorePath { get; init; }

    public required string LogPath { get; init; }

    public int ParentProcessId { get; init; }
}
