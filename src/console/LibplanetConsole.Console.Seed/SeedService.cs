using LibplanetConsole.Common;
using LibplanetConsole.Seed;

namespace LibplanetConsole.Console.Seed;

internal sealed class SeedService(IApplicationOptions options) : ISeedService
{
    private readonly PrivateKey _seedNodePrivateKey = new();
    private SeedNode? _blocksyncSeedNode;
    private SeedNode? _consensusSeedNode;

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
        var blocksyncPort = options.BlocksyncPort;
        var consensusPort = options.ConsensusPort;
        var appProtocolVersion = AppProtocolVersion.FromToken(options.AppProtocolVersion);
        _blocksyncSeedNode = new SeedNode(
            "Blocksync",
            new()
            {
                PrivateKey = _seedNodePrivateKey,
                Port = blocksyncPort is 0 ? PortUtility.NextPort() : blocksyncPort,
                AppProtocolVersion = appProtocolVersion,
            });
        _consensusSeedNode = new SeedNode(
            "Consensus",
            new()
            {
                PrivateKey = _seedNodePrivateKey,
                Port = consensusPort is 0 ? PortUtility.NextPort() : consensusPort,
                AppProtocolVersion = appProtocolVersion,
            });
        await _blocksyncSeedNode.StartAsync(cancellationToken);
        await _consensusSeedNode.StartAsync(cancellationToken);
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
