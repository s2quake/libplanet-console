namespace LibplanetConsole.Client;

public sealed record class ApplicationOptions
{
    public ApplicationOptions(int port, PrivateKey privateKey)
    {
        Port = port;
        PrivateKey = privateKey;
    }

    public int Port { get; }

    public PrivateKey PrivateKey { get; }

    public int ParentProcessId { get; init; }

    public EndPoint? NodeEndPoint { get; init; }

    public string LogPath { get; init; } = string.Empty;

    public bool NoREPL { get; init; }
}
