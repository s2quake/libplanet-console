using System.ComponentModel.Composition;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Nodes.Services;

[Export(typeof(ILocalService))]
internal sealed class NodeService : LocalService<INodeService, INodeCallback>, INodeService
{
    private readonly Node _node;

    [ImportingConstructor]
    public NodeService(Node node)
    {
        _node = node;
        _node.Started += (s, e) => Callback.OnStarted(_node.Info);
        _node.Stopped += (s, e) => Callback.OnStopped();
    }

    public async Task<NodeInfo> GetInfoAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        return _node.Info;
    }

    public async Task<NodeInfo> StartAsync(CancellationToken cancellationToken)
    {
        await _node.StartAsync(cancellationToken);
        return _node.Info;
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => _node.StopAsync(cancellationToken);
}
