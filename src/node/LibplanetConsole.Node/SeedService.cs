using LibplanetConsole.Seed;
using Microsoft.Extensions.Hosting;
using static LibplanetConsole.Common.EndPointUtility;

namespace LibplanetConsole.Node;

internal sealed class SeedService(ApplicationOptions options) : ISeedService, IHostedService
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

    async Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        if (CompareEndPoint(options.SeedEndPoint, options.EndPoint) is true)
        {
            _blocksyncSeedNode = new SeedNode(new()
            {
                PrivateKey = _seedNodePrivateKey,
                EndPoint = NextEndPoint(),
            });
            _consensusSeedNode = new SeedNode(new()
            {
                PrivateKey = _seedNodePrivateKey,
                EndPoint = NextEndPoint(),
            });
            await _blocksyncSeedNode.StartAsync(cancellationToken);
            await _consensusSeedNode.StartAsync(cancellationToken);
        }
    }

    async Task IHostedService.StopAsync(CancellationToken cancellationToken)
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