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
    private bool _isDisposed;
    private SwarmHost _currentSwarmHost;
    private readonly SwarmHost[] _swarmHosts;

    [ImportingConstructor]
    public SwarmHostCollection(ApplicationOptions options)
        : this(CreatePrivateKeys(options.SwarmCount))
    {
    }

    public SwarmHostCollection()
        : this(CreatePrivateKeys(5))
    {
    }

    public SwarmHostCollection(PrivateKey[] validators)
    {
        var portQueue = new Queue<int>(GetRandomUnusedPorts(validators.Length * 2));
        var validatorKeys = validators.Select(item => item.PublicKey).ToArray();
        var swarmHosts = new SwarmHost[validators.Length];
        var peers = new BoundPeer[validators.Length];
        var consensusPeers = new BoundPeer[validators.Length];
        for (var i = 0; i < validators.Length; i++)
        {
            peers[i] = new BoundPeer(validators[i].PublicKey, new DnsEndPoint($"{IPAddress.Loopback}", portQueue.Dequeue()));
            consensusPeers[i] = new BoundPeer(validators[i].PublicKey, new DnsEndPoint($"{IPAddress.Loopback}", portQueue.Dequeue()));
        }
        for (var i = 0; i < validators.Length; i++)
        {
            var blockChain = BlockChainUtility.CreateBlockChain($"Swarm{i}", validatorKeys);
            swarmHosts[i] = new SwarmHost(index: i, validators, peers, consensusPeers);
        }
        _swarmHosts = swarmHosts;
        _currentSwarmHost = swarmHosts[0];
    }

    public SwarmHost CurrentSwarmHost
    {
        get => _currentSwarmHost;
        set
        {
            if (_swarmHosts.Contains(value) == false)
                throw new ArgumentException(nameof(value));
            _currentSwarmHost = value;
        }
    }

    public int Count => _swarmHosts.Length;

    public SwarmHost this[int index] => _swarmHosts[index];

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed == true)
            throw new ObjectDisposedException($"{this}");

        for (var i = _swarmHosts.Length - 1; i >= 0; i--)
        {
            var item = _swarmHosts[i]!;
            await item.DisposeAsync();
        }
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }

    public bool Contains(SwarmHost item) => _swarmHosts.Contains(item);

    public int IndexOf(SwarmHost item)
    {
        for (var i = 0; i < _swarmHosts.Length; i++)
        {
            if (Equals(item, _swarmHosts[i]) == true)
                return i;
        }
        return -1;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await Task.WhenAll(_swarmHosts.Select(item => item.StartAsync(cancellationToken)));
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

    private static PrivateKey[] CreatePrivateKeys(int count)
    {
        var keyList = new List<PrivateKey>(count);
        for (var i = 0; i < count; i++)
        {
            keyList.Add(new PrivateKey());
        }
        return keyList.ToArray();
    }

    #region IEnumerable

    IEnumerator<SwarmHost> IEnumerable<SwarmHost>.GetEnumerator()
    {
        foreach (var item in _swarmHosts)
        {
            yield return item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => _swarmHosts.GetEnumerator();

    #endregion
}
