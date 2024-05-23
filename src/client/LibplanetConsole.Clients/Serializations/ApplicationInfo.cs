namespace LibplanetConsole.Clients.Serializations;

public sealed record class ApplicationInfo
{
    public string EndPoint { get; init; } = string.Empty;

    public string NodeEndPoint { get; init; } = string.Empty;

    public string LogPath { get; init; } = string.Empty;
}
