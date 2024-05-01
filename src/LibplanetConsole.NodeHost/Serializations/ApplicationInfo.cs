using LibplanetConsole.NodeServices.Serializations;

namespace LibplanetConsole.NodeHost.Serializations;

public readonly record struct ApplicationInfo
{
    public string EndPoint { get; init; }

    public NodeInfo NodeInfo { get; init; }
}
