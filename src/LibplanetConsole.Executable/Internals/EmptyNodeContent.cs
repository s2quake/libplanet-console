using System.ComponentModel.Composition;

namespace LibplanetConsole.Executable.Internals;

[Export(typeof(INodeContent))]
[method: ImportingConstructor]
internal sealed class EmptyNodeContent(INode node)
    : NodeContentBase(node), IDisposable
{
    public void Dispose()
    {
        int wqer = 0;
    }
}
