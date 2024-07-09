using System.ComponentModel.Composition;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Databases;
using LibplanetConsole.Databases.Serializations;
using LibplanetConsole.Databases.Services;

namespace LibplanetConsole.Nodes.Databases.Services;

[Export(typeof(ILocalService))]
internal sealed class DatabaseNodeService : LocalService<IDatabaseService, IDatabaseCallback>,
    IDatabaseService, IDisposable
{
    private readonly DatabaseNode _explorerNode;

    [ImportingConstructor]
    public DatabaseNodeService(DatabaseNode explorerNode)
    {
        _explorerNode = explorerNode;
        _explorerNode.Started += DatabaseNode_Started;
        _explorerNode.Stopped += DatabaseNode_Stopped;
    }

    public void Dispose()
    {
        _explorerNode.Started -= DatabaseNode_Started;
        _explorerNode.Stopped -= DatabaseNode_Stopped;
    }

    public async Task<DatabaseInfo> GetInfoAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        return _explorerNode.Info;
    }

    public async Task<DatabaseInfo> StartAsync(
        DatabaseOptions options, CancellationToken cancellationToken)
    {
        await _explorerNode.StartAsync(options, cancellationToken);
        return _explorerNode.Info;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _explorerNode.StopAsync(cancellationToken);
    }

    private void DatabaseNode_Started(object? sender, EventArgs e)
    {
        Callback.OnStarted(_explorerNode.Info);
    }

    private void DatabaseNode_Stopped(object? sender, EventArgs e)
    {
        Callback.OnStopped();
    }
}
