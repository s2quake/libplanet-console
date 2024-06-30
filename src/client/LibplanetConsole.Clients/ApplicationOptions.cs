using LibplanetConsole.Common;

namespace LibplanetConsole.Clients;

public sealed record class ApplicationOptions
{
    public ApplicationOptions(AppEndPoint endPoint, AppPrivateKey privateKey)
    {
        EndPoint = endPoint;
        PrivateKey = privateKey;
    }

    public AppEndPoint EndPoint { get; }

    public AppPrivateKey PrivateKey { get; }

    public int ParentProcessId { get; init; }

    public bool IsSeed { get; init; }

    public AppEndPoint? NodeEndPoint { get; init; }

    public string LogPath { get; init; } = string.Empty;

    public bool NoREPL { get; init; }
}
