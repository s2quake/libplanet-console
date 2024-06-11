namespace LibplanetConsole.Consoles;

public abstract class NodeContentBase(INode node, string name) : INodeContent
{
    public NodeContentBase(INode node)
        : this(node, node.GetType().Name)
    {
    }

    public INode Node { get; } = node;

    public string Name { get; } = name;
}
