using LibplanetConsole.Options;

namespace LibplanetConsole.Console;

public sealed class ApplicationOptions : OptionsBase<ApplicationOptions>
{
    public const int SeedBlocksyncPortIncrement = 6;
    public const int SeedConsensusPortIncrement = 7;

    public required int Port { get; init; }

    public NodeOptions[] Nodes { get; init; } = [];

    public ClientOptions[] Clients { get; init; } = [];

    public byte[] Genesis { get; init; } = [];

    public string LogPath { get; init; } = string.Empty;

    public bool NoProcess { get; init; }

    public bool Detach { get; init; }

    public bool NewWindow { get; init; }
}
