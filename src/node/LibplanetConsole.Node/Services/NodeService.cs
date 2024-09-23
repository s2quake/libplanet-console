using System.ComponentModel.Composition;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using Serilog;

namespace LibplanetConsole.Node.Services;

[Export(typeof(ILocalService))]
internal sealed class NodeService : LocalService<INodeService, INodeCallback>, INodeService
{
    private readonly Node _node;
    private readonly ILogger _logger;

    [ImportingConstructor]
    public NodeService(Node node, ILogger logger)
    {
        _node = node;
        _logger = logger;
        _node.Started += (s, e) => Callback.OnStarted(_node.Info);
        _node.Stopped += (s, e) => Callback.OnStopped();
    }

    public async Task<NodeInfo> GetInfoAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        return _node.Info;
    }

    public async Task<NodeInfo> StartAsync(string seedEndPoint, CancellationToken cancellationToken)
    {
        _node.SeedEndPoint = AppEndPoint.Parse(seedEndPoint);
        await _node.StartAsync(cancellationToken);
        _logger.Information("Node started.");
        return _node.Info;
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => _node.StopAsync(cancellationToken);
}