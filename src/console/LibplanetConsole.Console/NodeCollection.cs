using System.Collections;
using System.Collections.Specialized;
using LibplanetConsole.Alias;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Console;

internal sealed class NodeCollection(
    IServiceProvider serviceProvider,
    IApplicationOptions options)
    : ConsoleContentBase("nodes"), IEnumerable<Node>, INodeCollection
{
    private static readonly object LockObject = new();
    private readonly List<Node> _nodeList = new(options.Nodes.Length);
    private readonly ILogger _logger = serviceProvider.GetLogger<NodeCollection>();
    private Node? _current;

    public event EventHandler? CurrentChanged;

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public Node? Current
    {
        get => _current;
        set
        {
            if (value is not null && _nodeList.Contains(value) == false)
            {
                throw new ArgumentException(
                    message: $"'{value}' is not included in the collection.",
                    paramName: nameof(value));
            }

            _current = value;
            CurrentChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public int Count => _nodeList.Count;

    INode? INodeCollection.Current
    {
        get => Current;
        set
        {
            if (value is not Node node)
            {
                throw new ArgumentException(
                    message: $"'{value}' is not included in the collection.",
                    paramName: nameof(value));
            }

            Current = node;
        }
    }

    public Node this[int index] => _nodeList[index];

    public Node this[Address address] => _nodeList.Single(item => item.Address == address);

    INode INodeCollection.this[int index] => this[index];

    INode INodeCollection.this[Address address] => this[address];

    public bool Contains(Node item) => _nodeList.Contains(item);

    public bool Contains(Address address) => _nodeList.Exists(item => item.Address == address);

    public int IndexOf(Node item)
    {
        for (var i = 0; i < _nodeList.Count; i++)
        {
            if (Equals(item, _nodeList[i]) == true)
            {
                return i;
            }
        }

        return -1;
    }

    public int IndexOf(Address address)
    {
        for (var i = 0; i < _nodeList.Count; i++)
        {
            if (Equals(address, _nodeList[i].Address) == true)
            {
                return i;
            }
        }

        return -1;
    }

    public async Task<Node> AddNewAsync(
        AddNewNodeOptions newOptions, CancellationToken cancellationToken)
    {
        var node = NodeFactory.CreateNew(serviceProvider, newOptions.NodeOptions);
        InsertNode(node);
        if (newOptions.ProcessOptions is { } processOptions)
        {
            await node.StartProcessAsync(processOptions, cancellationToken);
        }

        return node;
    }

    public async Task AttachAsync(
        AttachOptions options, CancellationToken cancellationToken)
    {
        var node = this[options.Address];
        if (node.IsAttached is true)
        {
            throw new InvalidOperationException("The node is already attached.");
        }

        node.Url = options.Url;
        await node.AttachAsync(cancellationToken);
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        for (var i = 0; i < _nodeList.Count; i++)
        {
            try
            {
                var node = _nodeList[i];
                if (options.ProcessOptions is { } processOptions)
                {
                    await node.StartProcessAsync(processOptions, cancellationToken);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while initializing node.");
            }
        }
    }

    async Task<INode> INodeCollection.AddNewAsync(
        AddNewNodeOptions newOptions, CancellationToken cancellationToken)
        => await AddNewAsync(newOptions, cancellationToken);

    bool INodeCollection.Contains(INode item) => item switch
    {
        Node node => Contains(node),
        _ => false,
    };

    int INodeCollection.IndexOf(INode item) => item switch
    {
        Node node => IndexOf(node),
        _ => -1,
    };

    IEnumerator<Node> IEnumerable<Node>.GetEnumerator()
        => _nodeList.GetEnumerator();

    IEnumerator<INode> IEnumerable<INode>.GetEnumerator()
        => _nodeList.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _nodeList.GetEnumerator();

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        try
        {
            for (var i = 0; i < _nodeList.Capacity; i++)
            {
                var node = NodeFactory.CreateNew(serviceProvider, options.Nodes[i]);
                InsertNode(node);
            }

            Current = _nodeList.FirstOrDefault();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while starting nodes.");
        }

        await Task.CompletedTask;
    }

    protected override async Task OnStopAsync(CancellationToken cancellationToken)
    {
        for (var i = _nodeList.Count - 1; i >= 0; i--)
        {
            var node = _nodeList[i]!;
            node.Disposed -= Node_Disposed;
            await NodeFactory.DisposeScopeAsync(node);
            _logger.LogDebug("Disposed a client: {Address}", node.Address);
        }
    }

    private void Node_Disposed(object? sender, EventArgs e)
    {
        if (sender is Node node)
        {
            RemoveNode(node);
        }
    }

    private void InsertNode(Node node)
    {
        lock (LockObject)
        {
            var action = NotifyCollectionChangedAction.Add;
            var index = _nodeList.Count;
            var args = new NotifyCollectionChangedEventArgs(action, node, index);
            var aliases = serviceProvider.GetRequiredService<AliasCollection>();
            var aliasInfo = new AliasInfo
            {
                Alias = $"node-{index}",
                Address = node.Address,
                Tags = ["node", "temp"],
            };
            aliases.Add(aliasInfo);
            _nodeList.Add(node);
            _logger.LogDebug("Node is inserted into the collection: {Address}", node.Address);
            node.Disposed += Node_Disposed;
            CollectionChanged?.Invoke(this, args);
        }
    }

    private void RemoveNode(Node node)
    {
        lock (LockObject)
        {
            var action = NotifyCollectionChangedAction.Remove;
            var index = _nodeList.IndexOf(node);
            var args = new NotifyCollectionChangedEventArgs(action, node, index);
            var aliases = serviceProvider.GetRequiredService<AliasCollection>();
            node.Disposed -= Node_Disposed;
            _nodeList.RemoveAt(index);
            aliases.Remove(node.Address);
            _logger.LogDebug("Node is removed from the collection: {Address}", node.Address);
            CollectionChanged?.Invoke(this, args);
            if (_current == node)
            {
                Current = _nodeList.FirstOrDefault();
            }
        }
    }
}
