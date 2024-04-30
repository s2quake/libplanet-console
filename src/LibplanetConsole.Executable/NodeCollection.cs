using System.Collections;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.Net;
using Libplanet.Crypto;
using Libplanet.Net;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Exceptions;
using LibplanetConsole.Frameworks;
using LibplanetConsole.NodeServices;

namespace LibplanetConsole.Executable;

[Export]
[Export(typeof(IApplicationService))]
[method: ImportingConstructor]
internal sealed class NodeCollection(ApplicationOptions options)
    : IEnumerable<INode>, IApplicationService
{
    private static readonly object LockObject = new();
    private readonly ApplicationOptions _options = options;
    private readonly List<Node> _nodeList = new(options.NodeCount);
    private Node? _genesisNode;
    private INode? _current;
    private bool _isDisposed;

    public event EventHandler? CurrentChanged;

    public INode? Current
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

    public INode this[int index] => _nodeList[index];

    public INode this[Address address] => _nodeList.Single(item => item.Address == address);

    public bool Contains(INode item) => _nodeList.Contains(item);

    public bool Contains(Address address) => _nodeList.Any(item => item.Address == address);

    public int IndexOf(INode item)
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

    public Task<INode> AddNewAsync(CancellationToken cancellationToken)
        => AddNewAsync(new(), cancellationToken);

    public async Task<INode> AddNewAsync(PrivateKey privateKey, CancellationToken cancellationToken)
    {
        if (_genesisNode == null)
        {
            throw new InvalidOperationException("Genesis node is not set.");
        }

        var genesisPublicKey = _genesisNode.PrivateKey.PublicKey;
        var nodeOptions = new NodeOptions
        {
            SeedPeer = new BoundPeer(genesisPublicKey, _genesisNode.SwarmEndPoint),
            ConsensusSeedPeer = new BoundPeer(genesisPublicKey, _genesisNode.ConsensusEndPoint),
        };
        var endPoint = DnsEndPointUtility.Next();
        _ = new NodeProcess(endPoint, privateKey);
        var node = new Node(privateKey, endPoint);
        await node.StartAsync(nodeOptions, cancellationToken);
        lock (LockObject)
        {
            _nodeList.Add(node);
        }

        node.Disposed += Node_Disposed;
        return node;
    }

    public async Task<INode> AttachAsync(
        EndPoint endPoint, PrivateKey privateKey, CancellationToken cancellationToken)
    {
        if (_genesisNode == null && privateKey != GenesisOptions.DefaultGenesisKey)
        {
            throw new InvalidOperationException("Genesis node is not set.");
        }

        var nodeOptions = _genesisNode?.NodeOptions ?? new NodeOptions();
        var node = new Node(privateKey, endPoint);
        await node.StartAsync(nodeOptions, cancellationToken);
        _nodeList.Add(node);
        if (privateKey == GenesisOptions.DefaultGenesisKey)
        {
            _genesisNode = node;
            Current = node;
        }

        return node;
    }

    async Task IApplicationService.InitializeAsync(
        IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        if (_options.NodeCount > 0)
        {
            await AddGenesisNodeAsync(cancellationToken);
            await Parallel.ForAsync(1, _options.NodeCount, async (index, cancellationToken) =>
            {
                var privateKey = ProposePrivateKey(index);
                await AddNewAsync(privateKey, cancellationToken);
            });
        }

        static PrivateKey ProposePrivateKey(int index)
        {
            if (index < GenesisOptions.Validators.Length)
            {
                return GenesisOptions.Validators[index];
            }

            return new PrivateKey();
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

    private async Task<Node> AddGenesisNodeAsync(CancellationToken cancellationToken)
    {
        var nodeOptions = new NodeOptions();
        var endPoint = DnsEndPointUtility.Next();
        var privateKey = GenesisOptions.DefaultGenesisKey;
        _ = new NodeProcess(endPoint, privateKey);
        var node = new Node(privateKey, endPoint);
        await node.StartAsync(nodeOptions, cancellationToken);
        _nodeList.Add(node);
        _genesisNode = node;
        node.Disposed += Node_Disposed;
        Current = node;
        return node;
    }

    private void Node_Disposed(object? sender, EventArgs e)
    {
        if (sender is Node node)
        {
            _nodeList.Remove(node);
            if (_current == node)
            {
                Current = _nodeList.FirstOrDefault();
            }
        }
    }
}
