using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Exceptions;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Consoles.Services;
using LibplanetConsole.Frameworks;
using Serilog;

namespace LibplanetConsole.Consoles;

[Dependency(typeof(SeedService))]
[method: ImportingConstructor]
internal sealed class NodeCollection(
    ApplicationBase application, AppPrivateKey[] privateKeys)
    : IEnumerable<Node>, INodeCollection, IApplicationService, IAsyncDisposable
{
    private static readonly object LockObject = new();
    private readonly ApplicationBase _application = application;
    private readonly List<Node> _nodeList = new(privateKeys.Length);
    private readonly ILogger _logger = application.GetService<ILogger>();
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

    public Node this[AppAddress address] => _nodeList.Single(item => item.Address == address);

    INode INodeCollection.this[int index] => this[index];

    INode INodeCollection.this[AppAddress address] => this[address];

    public bool Contains(Node item) => _nodeList.Contains(item);

    public bool Contains(AppAddress address) => _nodeList.Exists(item => item.Address == address);

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

    public int IndexOf(AppAddress address)
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

    public async Task<Node> AddNewAsync(AddNewOptions options, CancellationToken cancellationToken)
    {
        var node = CreateNew(options.PrivateKey);
        if (options.NoProcess != true)
        {
            var nodeProcess = node.CreateProcess();
            nodeProcess.Detach = options.Detach;
            nodeProcess.ManualStart = options.Detach != true;
            nodeProcess.NewWindow = options.NewWindow;
            await nodeProcess.RunAsync(cancellationToken);
        }

        if (options.NoProcess != true && options.Detach != true)
        {
            await node.AttachAsync(cancellationToken);
        }

        if (node.IsAttached == true && options.ManualStart != true)
        {
            await node.StartAsync(cancellationToken);
        }

        InsertNode(node);
        return node;
    }

    async Task IApplicationService.InitializeAsync(
        IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var info = _application.Info;
        await Parallel.ForAsync(0, _nodeList.Capacity, cancellationToken, BodyAsync);
        Current = _nodeList.FirstOrDefault();

        async ValueTask BodyAsync(int index, CancellationToken cancellationToken)
        {
            var options = new AddNewOptions
            {
                PrivateKey = privateKeys[index],
                NoProcess = info.NoProcess,
                Detach = info.Detach,
                NewWindow = info.NewWindow,
                ManualStart = info.ManualStart,
            };
            await AddNewAsync(options, cancellationToken);
        }
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);

        for (var i = _nodeList.Count - 1; i >= 0; i--)
        {
            var item = _nodeList[i]!;
            await item.DisposeAsync();
        }

        _isDisposed = true;
        GC.SuppressFinalize(this);
    }

    async Task<INode> INodeCollection.AddNewAsync(
        AddNewOptions options, CancellationToken cancellationToken)
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

    private Node CreateNew(AppPrivateKey privateKey)
    {
        lock (LockObject)
        {
            var seedService = _application.GetService<SeedService>();
            var nodeOptions = new NodeOptions
            {
                Genesis = BlockUtility.SerializeBlock(_application.GenesisBlock),
                BlocksyncSeedPeer = seedService.BlocksyncSeedPeer,
                ConsensusSeedPeer = seedService.ConsensusSeedPeer,
            };
            return new Node(_application, privateKey, nodeOptions)
            {
                EndPoint = AppEndPoint.Next(),
            };
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
