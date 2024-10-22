using LibplanetConsole.Seed;
using static LibplanetConsole.Common.EndPointUtility;

namespace LibplanetConsole.Node;

internal sealed class SeedService(IApplicationOptions options) : ISeedService
{
    private readonly PrivateKey _seedNodePrivateKey = new();
    private SeedNode? _blocksyncSeedNode;
    private SeedNode? _consensusSeedNode;

    public bool IsRunning => _blocksyncSeedNode is not null && _consensusSeedNode is not null;

    public bool IsEnabled { get; } = GetEnabled(options);

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
        if (IsEnabled is false)
        {
            throw new InvalidOperationException("SeedService is disabled.");
        }

        var blocksyncSeedNode = new SeedNode(new()
        {
            PrivateKey = _seedNodePrivateKey,
            Port = options.Port + ApplicationOptions.SeedBlocksyncPortIncrement,
        });
        var consensusSeedNode = new SeedNode(new()
        {
            PrivateKey = _seedNodePrivateKey,
            Port = options.Port + ApplicationOptions.SeedConsensusPortIncrement,
        });
        await blocksyncSeedNode.StartAsync(cancellationToken);
        await consensusSeedNode.StartAsync(cancellationToken);
        _blocksyncSeedNode = blocksyncSeedNode;
        _consensusSeedNode = consensusSeedNode;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (IsEnabled is false)
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

    private static bool GetEnabled(IApplicationOptions options)
    {
        var localHost = GetLocalHost(options.Port);
        return CompareEndPoint(options.SeedEndPoint, localHost);
    }
}
