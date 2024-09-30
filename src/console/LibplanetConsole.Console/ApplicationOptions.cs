namespace LibplanetConsole.Console;

public sealed record class ApplicationOptions
{
    public ApplicationOptions(EndPoint endPoint)
    {
        EndPoint = endPoint;
    }

    public EndPoint EndPoint { get; }

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
