namespace LibplanetConsole.Node;

public sealed class NodeEventArgs(NodeInfo nodeInfo) : EventArgs
{
    public NodeInfo NodeInfo { get; } = nodeInfo;
}
