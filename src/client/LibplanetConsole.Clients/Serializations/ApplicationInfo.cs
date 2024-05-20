namespace LibplanetConsole.Clients.Serializations;

public readonly record struct ApplicationInfo
{
    public string EndPoint { get; init; }

    public string NodeEndPoint { get; init; }
}
