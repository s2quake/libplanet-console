namespace LibplanetConsole.Console;

public sealed class RunningNode : IDisposable
{
    private readonly INodeCollection _nodes;
    private bool _isDisposed;
    private INode? _node;
    private INode? _runningNode;

    public RunningNode(INodeCollection nodes)
    {
        _nodes = nodes;
        _nodes.CurrentChanged += Nodes_CurrentChanged;
        SetNode(_nodes.Current);
    }

    public event EventHandler? Started;

    public event EventHandler? Stopped;

    public INode Node
        => _runningNode ?? throw new InvalidOperationException("Node content is not initialized.");

    void IDisposable.Dispose()
    {
        if (_isDisposed is false)
        {
            SetNode(null);
            _nodes.CurrentChanged -= Nodes_CurrentChanged;
            _isDisposed = true;
            GC.SuppressFinalize(this);
        }
    }

    private void SetNode(INode? node)
    {
        var runningNode = _runningNode;

        if (_node is not null)
        {
            _node.Started -= Node_Started;
            _node.Stopped -= Node_Stopped;
            if (_runningNode is not null)
            {
                runningNode = null;
            }
        }

        _node = node;

        if (_node is not null)
        {
            _node.Started += Node_Started;
            _node.Stopped += Node_Stopped;
            if (_node.IsRunning is true)
            {
                runningNode = _node;
            }
        }

        if (runningNode != _runningNode)
        {
            if (_runningNode is not null)
            {
                Stopped?.Invoke(this, EventArgs.Empty);
            }

            _runningNode = runningNode;

            if (_runningNode is not null)
            {
                Started?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private void Nodes_CurrentChanged(object? sender, EventArgs e) => SetNode(_nodes.Current);

    private void Node_Started(object? sender, EventArgs e)
    {
        if (sender is INode node)
        {
            SetNode(node);
        }
    }

    private void Node_Stopped(object? sender, EventArgs e)
    {
        if (sender is INode node)
        {
            SetNode(node);
        }
    }
}
