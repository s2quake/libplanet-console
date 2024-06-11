namespace LibplanetConsole.Consoles;

public abstract class NodeContentBase(INode node, string name) : INodeContent
{
    public NodeContentBase(INode node)
        : this(node, string.Empty)
    {
    }

    public INode Node { get; } = node;

    public string Name => name != string.Empty ? name : GetType().Name;
}
