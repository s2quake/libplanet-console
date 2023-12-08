using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Libplanet.Crypto;

namespace OnBoarding.ConsoleHost;

[Export]
[method: ImportingConstructor]
sealed class SwarmHostCollection(UserCollection users) : IEnumerable<SwarmHost>, IAsyncDisposable
{
    private readonly OrderedDictionary _itemById = [];
    private readonly UserCollection _users = users;
    private bool _isDisposed;
    private readonly PublicKey[] _validatorKeys = [.. users.Select(item => item.PublicKey)];

    public int Count => _itemById.Count;

    public SwarmHost this[int index] => (SwarmHost)_itemById[index]!;

    public SwarmHost this[string key] => (SwarmHost)_itemById[key]!;

    public SwarmHost AddNew(User user)
    {
        ObjectDisposedException.ThrowIf(condition: _isDisposed, this);

        var validatorKeys = _validatorKeys;
        var peers = _users.Select(item => item.Peer).ToArray();
        var consensusPeers = _users.Select(item => item.ConsensusPeer).ToArray();
        var swarmHost = new SwarmHost(user, _users);
        _itemById.Add(swarmHost.Key, swarmHost);
        swarmHost.Disposed += Item_Disposed;
        return swarmHost;
    }

    public async ValueTask DisposeAsync()
    {
        ObjectDisposedException.ThrowIf(condition: _isDisposed, this);

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
        ObjectDisposedException.ThrowIf(condition: _isDisposed, this);
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

    public async Task InitializeAsync(Application application, CancellationToken cancellationToken)
    {
        if (application.GetService<UserCollection>() is { } users)
        {
            var swarmHosts = new SwarmHost[users.Count];
            for (var i = 0; i < users.Count; i++)
            {
                swarmHosts[i] = AddNew(users[i]);
                await swarmHosts[i].StartAsync(cancellationToken);
                Console.WriteLine($"{i}               {i}");
                // await Task.Delay(10000);
            }
            // await Task.WhenAll(swarmHosts.Select(item => item.StartAsync(cancellationToken)));
            // for (var i = 0; i < swarmHosts.Length; i++)
            // {
            //     var swarmHost = swarmHosts[i];
            //     var swarm = swarmHost.Target;
            //     var peers = swarmHosts.Where(item => item != swarmHost).Select(item => item.Target.AsPeer).ToArray();
            //     await swarm.AddPeersAsync(peers, TimeSpan.FromSeconds(1), cancellationToken);
            // }
        }
        else
        {
            throw new UnreachableException();
        }
    }

    private static int GetRandomUnusedPort()
    {
        var listener = CreateListener();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;

        static TcpListener CreateListener()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            return listener;
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
