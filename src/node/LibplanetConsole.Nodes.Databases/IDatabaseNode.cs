using LibplanetConsole.Databases;
using LibplanetConsole.Databases.Serializations;

namespace LibplanetConsole.Nodes.Databases;

public interface IDatabaseNode
{
    event EventHandler? Started;

    event EventHandler? Stopped;

    DatabaseInfo Info { get; }

    bool IsRunning { get; }

    ITableCollection Tables { get; }

    Task StartAsync(DatabaseOptions options, CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
