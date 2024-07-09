using System.Collections.Specialized;

namespace LibplanetConsole.Nodes.Databases;

public interface ITableCollection : IEnumerable<ITable>, INotifyCollectionChanged
{
    ITable this[string tableName] { get; }

    Task<ITable> CreateAsync(string tableName, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(string tableName, CancellationToken cancellationToken);
}
