using LibplanetConsole.Seed;
using Microsoft.Extensions.Options;
using static LibplanetConsole.Common.EndPointUtility;

namespace LibplanetConsole.Node;

internal sealed class SeedService(IOptions<ApplicationOptions> options) : ISeedService
{
    private readonly PrivateKey _seedNodePrivateKey = new();
    private readonly ApplicationOptions _options = options.Value;
    private readonly bool _isEnabled = GetEnabled(options);
    private SeedNode? _blocksyncSeedNode;
    private SeedNode? _consensusSeedNode;

    public bool IsRunning => _blocksyncSeedNode is not null && _consensusSeedNode is not null;

    public Task<SeedInfo> GetSeedAsync(
        PublicKey publicKey, CancellationToken cancellationToken)
    {
        if (_blocksyncSeedNode is null || _consensusSeedNode is null)
        {
            throw new InvalidOperationException("The SeedService is not running.");
        }

        var seedPeer = _blocksyncSeedNode.BoundPeer;
        var consensusSeedPeer = _consensusSeedNode.BoundPeer;
        var seedInfo = new SeedInfo
        {
            BlocksyncSeedPeer = seedPeer,
            ConsensusSeedPeer = consensusSeedPeer,
        };

        return Task.Run(() => seedInfo, cancellationToken);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_isEnabled is false)
        {
            throw new InvalidOperationException("SeedService is disabled.");
        }

        var blocksyncSeedNode = new SeedNode(new()
        {
            PrivateKey = _seedNodePrivateKey,
            Port = _options.Port + ApplicationOptions.SeedBlocksyncPortIncrement,
        });
        var consensusSeedNode = new SeedNode(new()
        {
            PrivateKey = _seedNodePrivateKey,
            Port = _options.Port + ApplicationOptions.SeedConsensusPortIncrement,
        });
        await blocksyncSeedNode.StartAsync(cancellationToken);
        await consensusSeedNode.StartAsync(cancellationToken);
        _blocksyncSeedNode = blocksyncSeedNode;
        _consensusSeedNode = consensusSeedNode;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_isEnabled is false)
        {
            throw new InvalidOperationException("SeedService is disabled.");
        }

        if (_blocksyncSeedNode is not null)
        {
            await _blocksyncSeedNode.StopAsync(cancellationToken: default);
            _blocksyncSeedNode = null;
        }

        if (_consensusSeedNode is not null)
        {
            await _consensusSeedNode.StopAsync(cancellationToken: default);
            _consensusSeedNode = null;
        }
    }

    private static bool GetEnabled(IOptions<ApplicationOptions> options)
    {
        var value = options.Value;
        var localHost = GetLocalHost(value.Port);
        return CompareEndPoint(value.SeedEndPoint, localHost);
    }
}
