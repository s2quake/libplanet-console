using System.ComponentModel.Composition;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Explorer;
using LibplanetConsole.Explorer.Services;

namespace LibplanetConsole.Node.Explorer.Services;

[Export(typeof(ILocalService))]
internal sealed class ExplorerService : LocalService<IExplorerService, IExplorerCallback>,
    IExplorerService, IDisposable
{
    private readonly Explorer _explorer;

    [ImportingConstructor]
    public ExplorerService(Explorer explorer)
    {
        _explorer = explorer;
        _explorer.Started += ExplorerNode_Started;
        _explorer.Stopped += ExplorerNode_Stopped;
    }

    public void Dispose()
    {
        _explorer.Started -= ExplorerNode_Started;
        _explorer.Stopped -= ExplorerNode_Stopped;
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
