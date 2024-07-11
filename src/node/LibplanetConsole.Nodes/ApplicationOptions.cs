using LibplanetConsole.Common;

namespace LibplanetConsole.Nodes;

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

    public bool ManualStart { get; init; } = false;

    public AppEndPoint? NodeEndPoint { get; init; }

    public string StorePath { get; init; } = string.Empty;

    public string LogPath { get; set; } = string.Empty;

    public AppPublicKey[] Validators { get; init; } = [];

    public bool NoREPL { get; init; }
}
