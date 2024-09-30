namespace LibplanetConsole.Console;

public readonly record struct ApplicationInfo
{
    public required EndPoint EndPoint { get; init; }

    public required string LogPath { get; init; }

    public bool NoProcess { get; init; }

    public bool Detach { get; init; }

    public bool NewWindow { get; init; }
}
