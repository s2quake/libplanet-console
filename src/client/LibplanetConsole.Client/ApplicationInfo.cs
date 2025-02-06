namespace LibplanetConsole.Client;

public readonly record struct ApplicationInfo
{
    public Uri? HubUrl { get; init; }

    public required string LogPath { get; init; }
}
