namespace LibplanetConsole.Console;

public abstract class NodeContentBase : INodeContent, IDisposable
{
    private readonly string _name;
    private bool _isDisposed;

    protected NodeContentBase(INode node, string name)
    {
        _name = name;
        Node = node;
        Node.Attached += Node_Attached;
        Node.Detached += Node_Detached;
        Node.Started += Node_Started;
        Node.Stopped += Node_Stopped;
    }

    protected NodeContentBase(INode node)
        : this(node, string.Empty)
    {
    }

    public INode Node { get; }

    public string Name => _name != string.Empty ? _name : GetType().Name;

    void IDisposable.Dispose()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        Node.Attached -= Node_Attached;
        Node.Detached -= Node_Detached;
        Node.Started -= Node_Started;
        Node.Stopped -= Node_Stopped;
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }

    protected virtual void OnNodeAttached()
    {
    }

    protected virtual void OnNodeDetached()
    {
    }

    protected virtual void OnNodeStarted()
    {
    }

    protected virtual void OnNodeStopped()
    {
    }

    private void Node_Attached(object? sender, EventArgs e) => OnNodeAttached();

    private void Node_Detached(object? sender, EventArgs e) => OnNodeDetached();

    private void Node_Started(object? sender, EventArgs e) => OnNodeStarted();

    private void Node_Stopped(object? sender, EventArgs e) => OnNodeStopped();
}
