using LibplanetConsole.Consoles;

namespace LibplanetConsole.ConsoleHost;

public abstract class NodeContentBase(INode node) : INodeContent
{
    public INode Node { get; } = node;
}
