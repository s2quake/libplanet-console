using LibplanetConsole.Common;

namespace LibplanetConsole.Node;

public sealed record class ApplicationOptions
{
    public ApplicationOptions(EndPoint endPoint, PrivateKey privateKey, byte[] genesis)
    {
        EndPoint = endPoint;
        PrivateKey = privateKey;
        Genesis = genesis;
    }

    public EndPoint EndPoint { get; }

    public PrivateKey PrivateKey { get; }

    public byte[] Genesis { get; }

    public int ParentProcessId { get; init; }

    public EndPoint? SeedEndPoint { get; init; }

    public string StorePath { get; init; } = string.Empty;

    public string LogPath { get; set; } = string.Empty;

    public string LibraryLogPath { get; set; } = string.Empty;

    public bool NoREPL { get; init; }

    public object[] Components { get; init; } = [];

    public IActionProvider? ActionProvider { get; init; }
}
