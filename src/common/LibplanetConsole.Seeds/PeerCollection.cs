﻿using System.Collections;
using System.Collections.Concurrent;
using Libplanet.Net.Transports;
using LibplanetConsole.Common;
using Serilog;

namespace LibplanetConsole.Seeds;

internal sealed class PeerCollection(SeedOptions seedOptions) : IEnumerable<Peer>
{
    private readonly ConcurrentDictionary<AppAddress, Peer> _infoByAddress = [];
    private readonly SeedOptions _seedOptions = seedOptions;
    private readonly ILogger _logger = Log.ForContext<Seed>();

    public int Count => _infoByAddress.Count;

    public void AddOrUpdate(AppPeer boundPeer, ITransport transport)
    {
        _infoByAddress.AddOrUpdate(
            key: boundPeer.Address,
            addValueFactory: _ =>
            {
                var peer = new Peer(transport, boundPeer)
                {
                    LifeTimeSpan = _seedOptions.PeerLifetime,
                };
                peer.Update();
                return peer;
            },
            updateValueFactory: (_, peer) =>
            {
                peer.Update();
                return peer;
            });
    }

    public async Task RefreshAsync(CancellationToken cancellationToken)
    {
        var peers = _infoByAddress.Values.ToArray();
        var pingTimeout = _seedOptions.PingTimeout;
        var updatedCount = 0;
        await Parallel.ForEachAsync(_infoByAddress.Values, cancellationToken, async (peer, ct) =>
        {
            if (await peer.PingAsync(pingTimeout, ct) == true)
            {
                Interlocked.Increment(ref updatedCount);
            }
            else
            {
                _infoByAddress.TryRemove(peer.Address, out _);
            }
        });
        _logger.Information(
            "Refreshing peers in table. (Total: {Total}, Candidate: {Candidate})",
            peers.Length,
            updatedCount);
    }

    public IEnumerator<Peer> GetEnumerator()
        => _infoByAddress.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _infoByAddress.Values.GetEnumerator();
}
