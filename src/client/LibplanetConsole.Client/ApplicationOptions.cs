using LibplanetConsole.Common;

namespace LibplanetConsole.Client;

public sealed record class ApplicationOptions
{
    public ApplicationOptions(AppEndPoint endPoint, PrivateKey privateKey)
    {
        EndPoint = endPoint;
        PrivateKey = privateKey;
    }

    public AppEndPoint EndPoint { get; }

    public PrivateKey PrivateKey { get; }

    public int ParentProcessId { get; init; }

    public AppEndPoint? NodeEndPoint { get; init; }

    public string LogPath { get; init; } = string.Empty;

    public bool NoREPL { get; init; }

    public object[] Components { get; init; } = [];
}
