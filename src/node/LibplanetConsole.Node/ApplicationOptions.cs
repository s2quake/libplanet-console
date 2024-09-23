using LibplanetConsole.Common;

namespace LibplanetConsole.Node;

public sealed record class ApplicationOptions
{
    public ApplicationOptions(AppEndPoint endPoint, AppPrivateKey privateKey, byte[] genesis)
    {
        EndPoint = endPoint;
        PrivateKey = privateKey;
        Genesis = genesis;
    }

    public AppEndPoint EndPoint { get; }

    public AppPrivateKey PrivateKey { get; }

    public byte[] Genesis { get; }

    public int ParentProcessId { get; init; }

    public AppEndPoint? SeedEndPoint { get; init; }

    public string StorePath { get; init; } = string.Empty;

    public string LogPath { get; set; } = string.Empty;

    public string LibraryLogPath { get; set; } = string.Empty;

    public bool NoREPL { get; init; }

    public object[] Components { get; init; } = [];
}
