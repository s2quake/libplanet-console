using System.Collections;
using System.Collections.Specialized;
using LibplanetConsole.Common.Exceptions;
using RocksDbSharp;

namespace LibplanetConsole.Nodes.Databases;

internal sealed class TableCollection : ITableCollection, IDisposable
{
    private readonly RocksDb _db;
    private readonly ColumnFamilyOptions _columnFamilyOptions;
    private readonly Dictionary<ColumnFamilyHandle, Table> _tableByHandle = [];
    private bool _isDisposed;

    public TableCollection(
        RocksDb db, ColumnFamilyOptions columnFamilyOptions, string[] tableNames)
    {
        _db = db;
        _columnFamilyOptions = columnFamilyOptions;
        foreach (var tableName in tableNames)
        {
            var columnFamilyHandle = _db.GetColumnFamily(tableName);
            _tableByHandle.Add(columnFamilyHandle, new Table(db, tableName, columnFamilyHandle));
        }
    }

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public Table this[string tableName]
    {
        get
        {
            if (_db.TryGetColumnFamily(tableName, out var columnFamilyHandle) == true)
            {
                return _tableByHandle[columnFamilyHandle];
            }

            throw new KeyNotFoundException($"The table '{tableName}' does not exist.");
        }
    }

    ITable ITableCollection.this[string tableName] => this[tableName];

    public void Dispose()
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);

        _tableByHandle.Clear();
        CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Reset));
        _isDisposed = true;
    }

    public async Task<Table> CreateAsync(string tableName, CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        if (_db.TryGetColumnFamily(tableName, out var columnFamilyHandle) == true)
        {
            throw new InvalidOperationException($"The table '{tableName}' already exists.");
        }

        columnFamilyHandle = _db.CreateColumnFamily(_columnFamilyOptions, tableName);
        await Task.CompletedTask;
        var table = new Table(_db, tableName, columnFamilyHandle);
        _tableByHandle.Add(columnFamilyHandle, table);
        return table;
    }

    public async Task<bool> DeleteAsync(string tableName, CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);

        if (_db.TryGetColumnFamily(tableName, out var columnFamilyHandle) == true)
        {
            _tableByHandle.Remove(columnFamilyHandle);
            return true;
        }

        await Task.CompletedTask;
        return false;
    }

    public IEnumerator<Table> GetEnumerator() => _tableByHandle.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _tableByHandle.Values.GetEnumerator();

    async Task<ITable> ITableCollection.CreateAsync(
        string tableName, CancellationToken cancellationToken)
        => await CreateAsync(tableName, cancellationToken);

    IEnumerator<ITable> IEnumerable<ITable>.GetEnumerator()
        => _tableByHandle.Values.GetEnumerator();
}
