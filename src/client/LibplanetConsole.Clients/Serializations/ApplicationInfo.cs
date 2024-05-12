namespace LibplanetConsole.Clients.Serializations;

public readonly record struct ApplicationInfo
{
    public string ServiceEndPoint { get; init; }

    public ClientInfo ClientInfo { get; init; }
}
