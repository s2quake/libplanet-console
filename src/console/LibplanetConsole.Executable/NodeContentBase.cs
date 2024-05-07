namespace LibplanetConsole.Executable;

public abstract class NodeContentBase(INode node) : INodeContent
{
    public INode Node { get; } = node;
}
