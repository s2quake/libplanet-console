using System.Collections;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.Net;
using System.Net.Sockets;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using Libplanet.Net;

namespace OnBoarding.ConsoleHost;

[Export]
[Export(typeof(IApplicationService))]
sealed class SwarmHostCollection : IEnumerable<SwarmHost>, IApplicationService
{
    private SwarmHost _current;
    private readonly SwarmHost[] _swarmHosts;
    private readonly BoundPeer _seedPeer;
    private readonly BoundPeer _consensusSeedPeer;
    private bool _isDisposed;

    [ImportingConstructor]
    public SwarmHostCollection(ApplicationOptions options)
        : this(CreatePrivateKeys(options.SwarmCount), options.StorePath)
    {
    }

    public SwarmHostCollection()
        : this(CreatePrivateKeys(4), storePath: string.Empty)
    {
    }

    public SwarmHostCollection(PrivateKey[] validators)
        : this(validators, storePath: string.Empty)
    {
    }

    public SwarmHostCollection(PrivateKey[] validators, string storePath)
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
            var privateKey = validators[i];
            var peer = peers[i];
            var consensusPeer = consensusPeers[i];
            var blockChain = CreateBlockChain($"Swarm{i}");
            swarmHosts[i] = new SwarmHost(privateKey, blockChain, peer, consensusPeer);
        }
        _swarmHosts = swarmHosts;
        _current = swarmHosts[0];
        _seedPeer = peers[0];
        _consensusSeedPeer = consensusPeers[0];

        BlockChain CreateBlockChain(string name)
        {
            if (storePath == string.Empty)
                return BlockChainUtility.CreateBlockChain(name, validatorKeys);
            return BlockChainUtility.CreateBlockChain(name, validatorKeys, storePath);
        }
    }

    public SwarmHost Current
    {
        get => _current;
        set
        {
            if (_swarmHosts.Contains(value) == false)
                throw new ArgumentException($"'{value}' is not included in the collection.", nameof(value));
            _current = value;
            CurrentChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public int Count => _swarmHosts.Length;

    public SwarmHost this[int index] => _swarmHosts[index];

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

    public event EventHandler? CurrentChanged;

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
            keyList.Add(PrivateKeyUtility.Create($"Swarm{i}"));
        }
        return keyList.ToArray();
    }

    #region IApplicationService

    async Task IApplicationService.InitializeAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        await Task.WhenAll(_swarmHosts.Select(item => item.StartAsync(_seedPeer, _consensusSeedPeer, cancellationToken)));
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
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

    #endregion

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
