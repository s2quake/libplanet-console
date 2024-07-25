using LibplanetConsole.Common;

namespace LibplanetConsole.Seeds;

public sealed class SeedNode(SeedNodeOptions seedNodeOptions)
{
    private readonly SeedNodeOptions _seedNodeOptions = seedNodeOptions;
    private CancellationTokenSource? _cancellationTokenSource;
    private Seed? _seed;

    public bool IsRunning => _cancellationTokenSource is not null;

    public AppPeer BoundPeer => new(
        _seedNodeOptions.PrivateKey.PublicKey, _seedNodeOptions.EndPoint);

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_cancellationTokenSource is not null)
        {
            throw new InvalidOperationException("Seed node is already running.");
        }

        var seedOptions = new SeedOptions
        {
            PrivateKey = _seedNodeOptions.PrivateKey,
            EndPoint = _seedNodeOptions.EndPoint,
        };

        _seed = new Seed(seedOptions);
        _cancellationTokenSource = new();
        await _seed.StartAsync(_cancellationTokenSource.Token);
        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_cancellationTokenSource is null)
        {
            throw new InvalidOperationException("Seed node is not running.");
        }

        await _cancellationTokenSource.CancelAsync();
        if (_seed is not null)
        {
            await _seed.StopAsync(cancellationToken);
            _seed = null;
        }

        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = null;
    }
}
