namespace LibplanetConsole.Nodes.Serializations;

public sealed record class ApplicationInfo
{
    public string EndPoint { get; init; } = string.Empty;

    public string NodeEndPoint { get; init; } = string.Empty;

    public string StorePath { get; init; } = string.Empty;

    public string LogPath { get; init; } = string.Empty;

    public int ParentProcessId { get; init; }
}
