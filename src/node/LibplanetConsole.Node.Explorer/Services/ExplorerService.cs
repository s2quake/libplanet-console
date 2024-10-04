using LibplanetConsole.Common.Services;
using LibplanetConsole.Explorer;
using LibplanetConsole.Explorer.Services;
using Microsoft.Extensions.Logging;

namespace LibplanetConsole.Node.Explorer.Services;

internal sealed class ExplorerService : LocalService<IExplorerService, IExplorerCallback>,
    IExplorerService, IDisposable
{
    private readonly INode _node;
    private readonly Explorer _explorer;
    private readonly ILogger _logger;

    public ExplorerService(INode node, Explorer explorer, ILogger<ExplorerService> logger)
    {
        _node = node;
        _explorer = explorer;
        _logger = logger;
        _explorer.Started += ExplorerNode_Started;
        _explorer.Stopped += ExplorerNode_Stopped;
        _logger.LogDebug("ExplorerService is created: {NodeAddress}", node.Address);
    }

    public void Dispose()
    {
        _explorer.Started -= ExplorerNode_Started;
        _explorer.Stopped -= ExplorerNode_Stopped;
        _logger.LogDebug("ExplorerService is disposed: {NodeAddress}", _node.Address);
    }

    public Task<ExplorerInfo> GetInfoAsync(CancellationToken cancellationToken)
        => Task.Run(() => _explorer.Info, cancellationToken);

    public Task<ExplorerInfo> StartAsync(
        ExplorerOptions options, CancellationToken cancellationToken)
        => _explorer.StartAsync(options, cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken)
        => _explorer.StopAsync(cancellationToken);

    private void ExplorerNode_Started(object? sender, EventArgs e)
        => Callback.OnStarted(_explorer.Info);

    private void ExplorerNode_Stopped(object? sender, EventArgs e)
        => Callback.OnStopped();
}
