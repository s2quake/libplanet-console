using Libplanet.Net;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Framework;
using LibplanetConsole.Seed;

namespace LibplanetConsole.Console.Services;

internal sealed class SeedService : LocalService<ISeedService>,
    ISeedService, IApplicationService, IAsyncDisposable
{
    private readonly PrivateKey _seedNodePrivateKey = new();
    private readonly SeedNode _blocksyncSeedNode;
    private readonly SeedNode _consensusSeedNode;
    private bool _isDisposed;

    public SeedService(IApplication application)
    {
        _blocksyncSeedNode = new SeedNode(new()
        {
            PrivateKey = _seedNodePrivateKey,
            EndPoint = EndPointUtility.NextEndPoint(),
        });
        _consensusSeedNode = new SeedNode(new()
        {
            PrivateKey = _seedNodePrivateKey,
            EndPoint = EndPointUtility.NextEndPoint(),
        });
    }

    public BoundPeer BlocksyncSeedPeer => _blocksyncSeedNode.BoundPeer;

    public BoundPeer ConsensusSeedPeer => _consensusSeedNode.BoundPeer;

    public async Task<SeedInfo> GetSeedAsync(
        PublicKey publicKey, CancellationToken cancellationToken)
    {
        var seedPeer = _blocksyncSeedNode.BoundPeer;
        var consensusSeedPeer = _consensusSeedNode.BoundPeer;
        var seedInfo = new SeedInfo
        {
            BlocksyncSeedPeer = seedPeer,
            ConsensusSeedPeer = consensusSeedPeer,
        };

        return await Task.Run(() => seedInfo, cancellationToken);
    }

    async Task IApplicationService.InitializeAsync(CancellationToken cancellationToken)
    {
        await _blocksyncSeedNode.StartAsync(cancellationToken);
        await _consensusSeedNode.StartAsync(cancellationToken);
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        if (_isDisposed is false)
        {
            await _blocksyncSeedNode.StopAsync(cancellationToken: default);
            await _consensusSeedNode.StopAsync(cancellationToken: default);
            _isDisposed = true;
        }
    }
}
