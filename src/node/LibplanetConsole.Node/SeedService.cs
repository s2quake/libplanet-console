using LibplanetConsole.Seed;

namespace LibplanetConsole.Node;

internal sealed class SeedService(ApplicationOptions options) : ISeedService
{
    private readonly PrivateKey _seedNodePrivateKey = new();
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
}
