using System.Collections;
using System.Collections.Specialized;
using LibplanetConsole.Common.Exceptions;
using LibplanetConsole.Console.Services;
using LibplanetConsole.Framework;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace LibplanetConsole.Console;

[Dependency(typeof(SeedService))]
internal sealed class NodeCollection(
    IServiceProvider serviceProvider, NodeOptions[] nodeOptions)
    : IEnumerable<Node>, INodeCollection, IApplicationService, IAsyncDisposable
{
    private static readonly object LockObject = new();
    private readonly List<Node> _nodeList = new(nodeOptions.Length);
    private readonly ILogger _logger = serviceProvider.GetRequiredService<ILogger>();
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private Node? _current;
    private bool _isDisposed;

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
        AddNewNodeOptions options, CancellationToken cancellationToken)
    {
        var node = CreateNew(options.NodeOptions);
        if (options.NoProcess != true)
        {
            await node.StartProcessAsync(options, cancellationToken);
        }

        if (options.NoProcess != true && options.Detach != true)
        {
            await node.AttachAsync(cancellationToken);
        }

        if (node.IsAttached is true && options.NodeOptions.SeedEndPoint is null)
        {
            await node.StartAsync(cancellationToken);
        }

        InsertNode(node);
        return node;
    }

    async Task IApplicationService.InitializeAsync(CancellationToken cancellationToken)
    {
        var info = serviceProvider.GetRequiredService<IApplication>().Info;
        await Parallel.ForAsync(0, _nodeList.Capacity, cancellationToken, BodyAsync);
        Current = _nodeList.FirstOrDefault();

        async ValueTask BodyAsync(int index, CancellationToken cancellationToken)
        {
            var options = new AddNewNodeOptions
            {
                NodeOptions = nodeOptions[index],
                NoProcess = info.NoProcess,
                Detach = info.Detach,
                NewWindow = info.NewWindow,
            };
            await AddNewAsync(options, cancellationToken);
        }
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);

        await _cancellationTokenSource.CancelAsync();
        _cancellationTokenSource.Dispose();
        for (var i = _nodeList.Count - 1; i >= 0; i--)
        {
            var item = _nodeList[i]!;
            await item.DisposeAsync();
        }

        _isDisposed = true;
        GC.SuppressFinalize(this);
    }

    async Task<INode> INodeCollection.AddNewAsync(
        AddNewNodeOptions options, CancellationToken cancellationToken)
        => await AddNewAsync(options, cancellationToken);

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

    internal Node RandomNode()
    {
        if (Count == 0)
        {
            throw new InvalidOperationException("No node is available.");
        }

        var nodeIndex = Random.Shared.Next(Count);
        return _nodeList[nodeIndex];
    }

    private Node CreateNew(NodeOptions nodeOptions)
    {
        lock (LockObject)
        {
            return new Node(serviceProvider, nodeOptions);
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
            _nodeList.Add(node);
            _logger.Debug("Node is inserted into the collection: {Address}", node.Address);
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
            node.Disposed -= Node_Disposed;
            _nodeList.RemoveAt(index);
            _logger.Debug("Node is removed from the collection: {Address}", node.Address);
            CollectionChanged?.Invoke(this, args);
            if (_current == node)
            {
                Current = _nodeList.FirstOrDefault();
            }
        }
    }
}
