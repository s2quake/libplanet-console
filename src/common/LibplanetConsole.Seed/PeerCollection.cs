using System.Collections;
using System.Collections.Concurrent;
using Libplanet.Net.Transports;
using Serilog;

namespace LibplanetConsole.Seed;

public sealed class PeerCollection : IEnumerable<Peer>
{
    private readonly ConcurrentDictionary<Address, Peer> _infoByAddress = [];
    private readonly SeedOptions _seedOptions;
    private readonly ILogger _logger = Log.ForContext<SeedNode>();

    internal PeerCollection(string name, SeedOptions seedOptions)
    {
        Name = name;
        _seedOptions = seedOptions;
    }

    public string Name { get; }

    public int Count => _infoByAddress.Count;

    public IEnumerator<Peer> GetEnumerator()
        => _infoByAddress.Values.GetEnumerator();

    public BoundPeer[] GetAlivePeers()
        => _infoByAddress.Values
            .Where(peer => peer.IsAlive)
            .Select(peer => peer.BoundPeer)
            .ToArray();

    IEnumerator IEnumerable.GetEnumerator()
        => _infoByAddress.Values.GetEnumerator();

    internal void AddOrUpdate(BoundPeer boundPeer)
    {
        _infoByAddress.AddOrUpdate(
            key: boundPeer.Address,
            addValueFactory: _ =>
            {
                var peer = new Peer(boundPeer)
                {
                    LifeTimeSpan = _seedOptions.PeerLifetime,
                };
                peer.Update();
                _logger.Debug("[{Name}] Added a new peer: {BoundPeer}", Name, boundPeer);
                return peer;
            },
            updateValueFactory: (_, peer) =>
            {
                peer.Update();
                _logger.Debug("[{Name}] Updated a peer: {BoundPeer}", Name, boundPeer);
                return peer;
            });
    }

    internal async Task RefreshAsync(ITransport transport, CancellationToken cancellationToken)
    {
        var peers = _infoByAddress.Values.ToArray();
        var pingTimeout = _seedOptions.PingTimeout;
        var updatedCount = 0;
        await Parallel.ForEachAsync(_infoByAddress.Values, cancellationToken, async (peer, ct) =>
        {
            if (await peer.PingAsync(transport, pingTimeout, ct) == true)
            {
                Interlocked.Increment(ref updatedCount);
                _logger.Debug("[{Name}] Pinged a peer: {BoundPeer}", Name, peer.BoundPeer);
            }
            else
            {
                _infoByAddress.TryRemove(peer.Address, out _);
                _logger.Debug("[{Name}] Failed to ping a peer: {BoundPeer}", Name, peer.BoundPeer);
            }
        });
        _logger.Debug(
            "[{Name}] Refreshing peers in table. (Total: {Total}, Candidate: {Candidate})",
            Name,
            peers.Length,
            updatedCount);
    }
}
