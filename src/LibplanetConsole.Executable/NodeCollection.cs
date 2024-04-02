using System.Collections;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using Libplanet.Crypto;
using Libplanet.Net;
using LibplanetConsole.Executable.Exceptions;

namespace LibplanetConsole.Executable;

[Export]
[Export(typeof(IApplicationService))]
sealed class NodeCollection : IEnumerable<Node>, IApplicationService
{
    private Node _current;
    private readonly Node[] _nodes;
    private readonly BoundPeer _seedPeer;
    private readonly BoundPeer _consensusSeedPeer;
    private bool _isDisposed;

    [ImportingConstructor]
    public NodeCollection(ApplicationOptions options)
        : this(CreatePrivateKeys(options.SwarmCount), options.StorePath)
    {
    }

    public NodeCollection()
        : this(CreatePrivateKeys(4), storePath: string.Empty)
    {
    }

    public NodeCollection(PrivateKey[] validators)
        : this(validators, storePath: string.Empty)
    {
    }

    public NodeCollection(PrivateKey[] validators, string storePath)
    {
        var validatorKeys = validators.Select(item => item.PublicKey).ToArray();
        var nodes = new Node[validators.Length];
        var peers = new BoundPeer[validators.Length];
        var consensusPeers = new BoundPeer[validators.Length];
        for (var i = 0; i < validators.Length; i++)
        {
            var privateKey = validators[i];
            var peer = peers[i];
            var consensusPeer = consensusPeers[i];
            nodes[i] = new Node($"Swarm{i}", privateKey, validatorKeys, storePath)
            {
                Identifier = $"n{i}",
            };
            peers[i] = nodes[i].Peer;
            consensusPeers[i] = nodes[i].ConsensusPeer;
        }
        _nodes = nodes;
        _current = nodes[0];
        _seedPeer = peers[0];
        _consensusSeedPeer = consensusPeers[0];
    }

    public Node Current
    {
        get => _current;
        set
        {
            if (_nodes.Contains(value) == false)
                throw new ArgumentException($"'{value}' is not included in the collection.", nameof(value));
            _current = value;
            CurrentChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public int Count => _nodes.Length;

    public Node this[int index] => _nodes[index];

    public Node this[Address address] => _nodes.Single(item => item.Address == address);

    public bool Contains(Node item) => _nodes.Contains(item);

    public bool Contains(Address address) => _nodes.Any(item => item.Address == address);

    public int IndexOf(Node item)
    {
        for (var i = 0; i < _nodes.Length; i++)
        {
            if (Equals(item, _nodes[i]) == true)
                return i;
        }
        return -1;
    }

    public int IndexOf(Address address)
    {
        for (var i = 0; i < _nodes.Length; i++)
        {
            if (Equals(address, _nodes[i].Address) == true)
                return i;
        }
        return -1;
    }

    public event EventHandler? CurrentChanged;

    private static PrivateKey[] CreatePrivateKeys(int count)
    {
        var keyList = new List<PrivateKey>(count);
        for (var i = 0; i < count; i++)
        {
            keyList.Add(PrivateKeyUtility.Create($"Swarm{i}"));
        }
        return [.. keyList];
    }

    #region IApplicationService

    async Task IApplicationService.InitializeAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        await Task.WhenAll(_nodes.Select(item => item.StartAsync(_seedPeer, _consensusSeedPeer, cancellationToken)));
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);

        for (var i = _nodes.Length - 1; i >= 0; i--)
        {
            var item = _nodes[i]!;
            await item.DisposeAsync();
        }
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }

    #endregion

    #region IEnumerable

    IEnumerator<Node> IEnumerable<Node>.GetEnumerator()
    {
        foreach (var item in _nodes)
        {
            yield return item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => _nodes.GetEnumerator();

    #endregion
}
