using LibplanetConsole.Common;

namespace LibplanetConsole.Consoles;

public sealed record class ApplicationOptions
{
    public ApplicationOptions(AppEndPoint endPoint)
    {
        EndPoint = endPoint;
    }

    public AppEndPoint EndPoint { get; }

    public NodeOptions[] Nodes { get; init; } = [];

    public ClientOptions[] Clients { get; init; } = [];

    public byte[] Genesis { get; init; } = [];

    public string LogPath { get; init; } = string.Empty;

    public string LibraryLogPath { get; init; } = string.Empty;

    public bool NoProcess { get; init; }

    public bool Detach { get; init; }

    public bool NewWindow { get; init; }

    public object[] Components { get; init; } = [];
}
