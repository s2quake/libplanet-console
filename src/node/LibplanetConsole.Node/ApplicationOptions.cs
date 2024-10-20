namespace LibplanetConsole.Node;

public sealed record class ApplicationOptions
{
    public const int BlocksyncPortIncrement = 4;
    public const int ConsensusPortIncrement = 5;
    public const int SeedBlocksyncPortIncrement = 6;
    public const int SeedConsensusPortIncrement = 7;

    public ApplicationOptions(int port, PrivateKey privateKey, byte[] genesis)
    {
        Port = port;
        PrivateKey = privateKey;
        Genesis = genesis;
    }

    public int Port { get; }

    public PrivateKey PrivateKey { get; }

    public byte[] Genesis { get; }

    public int ParentProcessId { get; init; }

    public EndPoint? SeedEndPoint { get; init; }

    public string StorePath { get; init; } = string.Empty;

    public string LogPath { get; set; } = string.Empty;

    public bool NoREPL { get; init; }

    public IActionProvider? ActionProvider { get; init; }
}
