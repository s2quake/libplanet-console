namespace LibplanetConsole.Nodes.Serializations;

public readonly record struct ApplicationInfo
{
    public string EndPoint { get; init; }

    public NodeInfo NodeInfo { get; init; }
}
