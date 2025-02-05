namespace LibplanetConsole.Node;

public readonly record struct ApplicationInfo
{
    public required Uri? HubUrl { get; init; }

    public required string StorePath { get; init; }

    public required string LogPath { get; init; }

    public int ParentProcessId { get; init; }

    public bool IsSingleNode { get; init; }
}
