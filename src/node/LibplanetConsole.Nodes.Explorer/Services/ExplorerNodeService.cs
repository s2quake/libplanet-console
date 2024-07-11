using System.ComponentModel.Composition;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Explorer;
using LibplanetConsole.Explorer.Services;

namespace LibplanetConsole.Nodes.Explorer.Services;

[Export(typeof(ILocalService))]
internal sealed class ExplorerNodeService : LocalService<IExplorerService, IExplorerCallback>,
    IExplorerService, IDisposable
{
    private readonly ExplorerNode _explorerNode;

    [ImportingConstructor]
    public ExplorerNodeService(ExplorerNode explorerNode)
    {
        _explorerNode = explorerNode;
        _explorerNode.Started += ExplorerNode_Started;
        _explorerNode.Stopped += ExplorerNode_Stopped;
    }

    public void Dispose()
    {
        _explorerNode.Started -= ExplorerNode_Started;
        _explorerNode.Stopped -= ExplorerNode_Stopped;
    }

    public Task<ExplorerInfo> GetInfoAsync(CancellationToken cancellationToken)
        => Task.Run(() => _explorerNode.Info, cancellationToken);

    public Task<ExplorerInfo> StartAsync(
        ExplorerOptions options, CancellationToken cancellationToken)
        => _explorerNode.StartAsync(options, cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken)
        => _explorerNode.StopAsync(cancellationToken);

    private void ExplorerNode_Started(object? sender, EventArgs e)
        => Callback.OnStarted(_explorerNode.Info);

    private void ExplorerNode_Stopped(object? sender, EventArgs e)
        => Callback.OnStopped();
}
