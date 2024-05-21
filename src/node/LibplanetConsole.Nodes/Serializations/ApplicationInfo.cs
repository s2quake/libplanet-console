namespace LibplanetConsole.Nodes.Serializations;

public readonly record struct ApplicationInfo
{
    public string EndPoint { get; init; }

    public string NodeEndPoint { get; init; }

    public string StorePath { get; init; }
}
