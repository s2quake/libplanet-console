using LibplanetConsole.Databases;

namespace LibplanetConsole.Nodes.Databases;

public interface ITable
{
    event EventHandler? Disposed;

    string Name { get; }

    Task PutAsync(PutOptions options, CancellationToken cancellationToken);

    Task PutRangeAsync(PutRangeOptions options, CancellationToken cancellationToken);

    Task<string> SeekAsync(SeekOptions options, CancellationToken cancellationToken);

    Task<KeyValue[]> SeekManyAsync(string find, string prefix, CancellationToken cancellationToken);

    Task DeleteAsync(string key, CancellationToken cancellationToken);

    Task<string[]> DeleteManyAsync(string[] keys, CancellationToken cancellationToken);
}
