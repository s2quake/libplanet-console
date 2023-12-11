using System.Collections;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Net;
using System.Net.Sockets;
using Libplanet.Crypto;
using Libplanet.Net;

namespace OnBoarding.ConsoleHost;

[Export]
sealed class SwarmHostCollection : IEnumerable<SwarmHost>, IAsyncDisposable
{
    private readonly OrderedDictionary _itemById = new();
    private readonly PrivateKey[] _validators;
    private readonly PublicKey[] _validatorKeys;
    private readonly SwarmHost _seedSwarmHost;
    private bool _isDisposed;
    private SwarmHost _currentSwarmHost;
    private Queue<int> _portQueue;

    [ImportingConstructor]
    public SwarmHostCollection(ApplicationOptions options)
        : this(CreatePrivateKeys(options.SwarmCount))
    {

    }

    public SwarmHostCollection()
        : this(CreatePrivateKeys(5))
    {
    }

    private static PrivateKey[] CreatePrivateKeys(int count)
    {
        var keyList = new List<PrivateKey>(count);
        for (var i = 0; i < count; i++)
        {
            keyList.Add(new PrivateKey());
        }
        return keyList.ToArray();
    }

    public SwarmHostCollection(PrivateKey[] validators)
    {
        _portQueue = new(GetRandomUnusedPorts(validators.Length * 2));
        _validators = validators;
        _validatorKeys = validators.Select(item => item.PublicKey).ToArray();
        _seedSwarmHost = new("Seed Swarm", _validators[0], _validatorKeys, _portQueue.Dequeue(), _portQueue.Dequeue());
        _itemById.Add(_seedSwarmHost.Key, _seedSwarmHost);
        _seedSwarmHost.Disposed += Item_Disposed;
        _currentSwarmHost = _seedSwarmHost;
    }

    public SwarmHost CurrentSwarmHost
    {
        get => _currentSwarmHost;
        set
        {
            if (_itemById.Contains(value.Key) == false)
                throw new ArgumentException(nameof(value));
            _currentSwarmHost = value;
        }
    }

    private static int[] GetRandomUnusedPorts(int count)
    {
        var ports = new int[count];
        var listeners = new TcpListener[count];
        for (var i = 0; i < count; i++)
        {
            listeners[i] = CreateListener();
            ports[i] = ((IPEndPoint)listeners[i].LocalEndpoint).Port;
        }
        for (var i = 0; i < count; i++)
        {
            listeners[i].Stop();
        }
        return ports;

        static TcpListener CreateListener()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            return listener;
        }
    }

    public int Count => _itemById.Count;

    public SwarmHost this[int index] => (SwarmHost)_itemById[index]!;

    public SwarmHost this[string key] => (SwarmHost)_itemById[key]!;

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed == true)
            throw new ObjectDisposedException($"{this}");

        for (var i = _itemById.Count - 1; i >= 0; i--)
        {
            var item = (SwarmHost)_itemById[i]!;
            item.Disposed -= Item_Disposed;
            await item.DisposeAsync();
        }
        _itemById.Clear();
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }

    public void Add(SwarmHost item)
    {
        if (_isDisposed == true)
            throw new ObjectDisposedException($"{this}");
        if (item.IsDisposed == true)
            throw new ArgumentException($"{nameof(item)} has already been disposed.");
        if (_itemById.Contains(item.Key) == true)
            throw new ArgumentException($"{nameof(item)} has already been included in collection");

        _itemById.Add(item.Key, item);
        item.Disposed += Item_Disposed;
    }

    public bool Contains(SwarmHost item) => _itemById.Contains(item.Key);

    public int IndexOf(SwarmHost item)
    {
        for (var i = 0; i < _itemById.Count; i++)
        {
            if (Equals(item, _itemById[i]) == true)
                return i;
        }
        return -1;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var swarmHostList = new List<SwarmHost>(_validators.Length)
        {
            _seedSwarmHost,
        };

        for (var i = 1; i < _validators.Length; i++)
        {
            var swarmHost = AddNew($"Swarm {i}", _validators[i], _portQueue.Dequeue(), _portQueue.Dequeue());
            swarmHostList.Add(swarmHost);
            swarmHost.StaticPeers = ImmutableHashSet.Create(_seedSwarmHost.Peer);
            swarmHost.ConsensusSeedPeers = ImmutableList.Create(_seedSwarmHost.ConsensusPeer);
        }
        var consensusPeers = swarmHostList.Select(item => item.ConsensusPeer).ToImmutableList();
        for (var i = 0; i < swarmHostList.Count; i++)
        {
            swarmHostList[i].ConsensusPeers = consensusPeers;
        }
        if (swarmHostList.Count > 1)
        {
            _seedSwarmHost.StaticPeers = ImmutableHashSet.Create(swarmHostList[1].Peer);
            _seedSwarmHost.ConsensusSeedPeers = ImmutableList.Create(swarmHostList[1].ConsensusPeer);
        }

        foreach (var item in swarmHostList)
        {
            await item.StartAsync(cancellationToken);
        }
    }

    private SwarmHost AddNew(string name, PrivateKey privateKey, int port, int consensusPort)
    {
        if (_isDisposed == true)
            throw new ObjectDisposedException($"{this}");

        var validatorKeys = _validatorKeys;
        var swarmHost = new SwarmHost(name, privateKey, validatorKeys, port, consensusPort);
        _itemById.Add(swarmHost.Key, swarmHost);
        swarmHost.Disposed += Item_Disposed;
        return swarmHost;
    }

    private void Item_Disposed(object? sender, EventArgs e)
    {
        if (sender is SwarmHost item)
        {
            item.Disposed -= Item_Disposed;
            _itemById.Remove(item.Key);
        }
    }

    #region IEnumerable

    IEnumerator<SwarmHost> IEnumerable<SwarmHost>.GetEnumerator()
    {
        foreach (var item in _itemById.Values)
        {
            if (item is SwarmHost obj)
            {
                yield return obj;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => _itemById.Values.GetEnumerator();

    #endregion
}
