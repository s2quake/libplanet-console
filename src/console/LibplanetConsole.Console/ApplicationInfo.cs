namespace LibplanetConsole.Console;

public readonly record struct ApplicationInfo
{
    public required string LogPath { get; init; }

    public required string GenesisHash { get; init; }

    public required string AppProtocolVersion { get; init; }

    public bool NoProcess { get; init; }

    public bool Detach { get; init; }

    public bool NewWindow { get; init; }
}
