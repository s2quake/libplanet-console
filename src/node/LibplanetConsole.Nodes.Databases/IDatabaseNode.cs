using LibplanetConsole.Databases;
using LibplanetConsole.Databases.Serializations;

namespace LibplanetConsole.Nodes.Databases;

public interface IDatabaseNode
{
    event EventHandler? Started;

    event EventHandler? Stopped;

    DatabaseInfo Info { get; }

    bool IsRunning { get; }

    Task StartAsync(DatabaseOptions options, CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);

    Task PutAsync(string key, string value, CancellationToken cancellationToken);

    Task PutRangeAsync(KeyValue[] keyValues, CancellationToken cancellationToken);

    Task<string> SeekAsync(string key, CancellationToken cancellationToken);

    Task<KeyValue[]> SeekManyAsync(string find, string prefix, CancellationToken cancellationToken);

    Task DeleteAsync(string key, CancellationToken cancellationToken);
}
