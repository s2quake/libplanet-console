using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Framework;
using LibplanetConsole.Seed;
using static LibplanetConsole.Common.EndPointUtility;

namespace LibplanetConsole.Node.Services;

[Export(typeof(ILocalService))]
[Export(typeof(IApplicationService))]
internal sealed class SeedService(ApplicationBase application)
    : LocalService<ISeedService>, ISeedService, IApplicationService, IAsyncDisposable
{
    private readonly PrivateKey _seedNodePrivateKey = new();
    private SeedNode? _blocksyncSeedNode;
    private SeedNode? _consensusSeedNode;

    public async Task<SeedInfo> GetSeedAsync(
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

        return await application.InvokeAsync(() => seedInfo, cancellationToken);
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
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

    async Task IApplicationService.InitializeAsync(
        IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        if (CompareEndPoint(application.Info.SeedEndPoint, application.Info.EndPoint) is true)
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
}
