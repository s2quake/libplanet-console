using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Net;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Exceptions;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Consoles.Services;
using LibplanetConsole.Frameworks;
using LibplanetConsole.Nodes;

namespace LibplanetConsole.Consoles;

[Dependency(typeof(SeedService))]
[method: ImportingConstructor]
internal sealed class NodeCollection(
    ApplicationBase application, PrivateKey[] privateKeys)
    : IEnumerable<Node>, INodeCollection, IApplicationService, IAsyncDisposable
{
    private static readonly object LockObject = new();
    private readonly ApplicationBase _application = application;
    private readonly List<Node> _nodeList = new(privateKeys.Length);
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

    public bool Contains(Address address) => _nodeList.Any(item => item.Address == address);

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

    public Task<Node> AddNewAsync(CancellationToken cancellationToken)
        => AddNewAsync(new(), cancellationToken);

    public async Task<Node> AddNewAsync(PrivateKey privateKey, CancellationToken cancellationToken)
    {
        var seedService = _application.GetService<SeedService>();
        var nodeOptions = new NodeOptions
        {
            GenesisOptions = _application.GenesisOptions,
            BlocksyncSeedPeer = seedService.BlocksyncSeedPeer,
            ConsensusSeedPeer = seedService.ConsensusSeedPeer,
        };
        var endPoint = DnsEndPointUtility.Next();
        var nodeProcessOptions = new NodeProcessOptions(endPoint, privateKey)
        {
            StoreDirectory = _application.Info.StoreDirectory,
            LogDirectory = _application.Info.LogDirectory,
        };
        _ = new NodeProcess(nodeProcessOptions);
        var node = CreateNew(privateKey, endPoint);
        await node.StartAsync(nodeOptions, cancellationToken);
        InsertNode(node);
        return node;
    }

    public async Task<Node> AttachAsync(
        EndPoint endPoint, PrivateKey privateKey, CancellationToken cancellationToken)
    {
        var seedService = _application.GetService<SeedService>();
        var nodeOptions = new NodeOptions
        {
            GenesisOptions = _application.GenesisOptions,
            BlocksyncSeedPeer = seedService.BlocksyncSeedPeer,
            ConsensusSeedPeer = seedService.ConsensusSeedPeer,
        };
        var node = CreateNew(privateKey, endPoint);
        await node.StartAsync(nodeOptions, cancellationToken);
        InsertNode(node);

        return node;
    }

    async Task IApplicationService.InitializeAsync(
        IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        await Parallel.ForAsync(0, _nodeList.Capacity, cancellationToken, BodyAsync);
        Current = _nodeList.FirstOrDefault();

        async ValueTask BodyAsync(int index, CancellationToken cancellationToken)
        {
            var privateKey = privateKeys[index];
            await AddNewAsync(privateKey, cancellationToken);
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
        PrivateKey privateKey, CancellationToken cancellationToken)
        => await AddNewAsync(privateKey, cancellationToken);

    async Task<INode> INodeCollection.AttachAsync(
        EndPoint endPoint, PrivateKey privateKey, CancellationToken cancellationToken)
        => await AttachAsync(endPoint, privateKey, cancellationToken);

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

    private Node CreateNew(PrivateKey privateKey, EndPoint endPoint)
    {
        lock (LockObject)
        {
            return new Node(_application, privateKey, endPoint);
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
            CollectionChanged?.Invoke(this, args);
            if (_current == node)
            {
                Current = _nodeList.FirstOrDefault();
            }
        }
    }
}
