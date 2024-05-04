using System.Collections.Immutable;
using Libplanet.Crypto;
using Libplanet.Net;

namespace LibplanetConsole.NodeServices.Seeds;

public sealed class SeedNode(SeedNodeOptions seedNodeOptions)
{
    private readonly SeedNodeOptions _seedNodeOptions = seedNodeOptions;
    private CancellationTokenSource? _cancellationTokenSource;
    private Seed? _seed;
    private Task? _seedTask;

    public bool IsRunning => _seedTask is not null;

    public BoundPeer BoundPeer => new(
        _seedNodeOptions.PrivateKey.PublicKey, _seedNodeOptions.EndPoint);

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_seedTask is not null)
        {
            throw new InvalidOperationException("Seed node is already running.");
        }

        _seed = new Seed(
            privateKey: _seedNodeOptions.PrivateKey,
            host: _seedNodeOptions.EndPoint.Host,
            port: _seedNodeOptions.EndPoint.Port,
            iceServers: [],
            appProtocolVersion: _seedNodeOptions.AppProtocolVersion,
            trustedAppProtocolVersionSigners: ImmutableHashSet<PublicKey>.Empty,
            maximumPeersToToRefresh: int.MaxValue,
            refreshInterval: TimeSpan.FromSeconds(120),
            peerLifetime: TimeSpan.FromSeconds(120),
            pingTimeout: TimeSpan.FromSeconds(120));
        _cancellationTokenSource = new();
        _seedTask = _seed.StartAsync(staticPeers: [], _cancellationTokenSource.Token);
        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_seedTask is null)
        {
            throw new InvalidOperationException("Seed node is not running.");
        }

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = null;
        if (_seed != null)
        {
            await _seed.StopAsync(TimeSpan.FromSeconds(5));
            _seed = null;
        }

        if (_seedTask != null)
        {
            await _seedTask;
            _seedTask = null;
        }
    }
}
