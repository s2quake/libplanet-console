using System.Text;
using DotNet.Globbing;
using LibplanetConsole.Common.Exceptions;
using LibplanetConsole.Databases;
using RocksDbSharp;

namespace LibplanetConsole.Nodes.Databases;

internal sealed class Table : ITable, IDisposable
{
    private readonly RocksDb _db;
    private readonly ColumnFamilyHandle _columnFamilyHandle;
    private bool _isDisposed;

    public Table(RocksDb db, string name, ColumnFamilyHandle columnFamilyHandle)
    {
        _db = db;
        Name = name;
        _columnFamilyHandle = columnFamilyHandle;
    }

    public event EventHandler? Disposed;

    public string Name { get; }

    public void Dispose()
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);

        _isDisposed = true;
        Disposed?.Invoke(this, EventArgs.Empty);
    }

    public async Task PutAsync(PutOptions options, CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);

        var writeOptions = new WriteOptions();
        var columnFamilyHandle = _columnFamilyHandle;
        var key = options.Key;
        var value = options.Value;
        _db.Put(key, value, columnFamilyHandle, writeOptions);
        await Task.CompletedTask;
    }

    public async Task PutRangeAsync(PutRangeOptions options, CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);

        var writeOptions = new WriteOptions();
        var columnFamilyHandle = _columnFamilyHandle;
        var writeBatch = new WriteBatch();
        var keyValues = options.KeyValues;
        for (var i = 0; i < keyValues.Length; i++)
        {
            var valuePair = keyValues[i];
            var key = Encoding.UTF8.GetBytes(valuePair.Key);
            var value = Encoding.UTF8.GetBytes(valuePair.Value);
            writeBatch.Put(key, value, columnFamilyHandle);
        }

        _db.Write(writeBatch, writeOptions);
        await Task.CompletedTask;
    }

    public async Task<string> SeekAsync(SeekOptions options, CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);

        var readOptions = new ReadOptions();
        var columnFamilyHandle = _columnFamilyHandle;
        var key = options.Key;
        var value = _db.Get(key, columnFamilyHandle, readOptions)
            ?? throw new InvalidOperationException($"Key '{key}' is not found.");
        await Task.CompletedTask;
        return value;
    }

    public async Task<KeyValue[]> SeekManyAsync(
        string find, string prefix, CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);

        var readOptions = new ReadOptions();
        using var iterator = _db.NewIterator(readOptions: readOptions);
        var keyValueList = new List<KeyValue>(10);
        var glob = Glob.Parse(find == string.Empty ? "*" : find);

        iterator.Seek(prefix);
        while (iterator.Valid() == true && iterator.StringKey().StartsWith(prefix) == true)
        {
            var key = iterator.StringKey();
            var value = iterator.StringValue();
            if (keyValueList.Count == keyValueList.Capacity)
            {
                keyValueList.Capacity += 10;
            }

            if (glob.IsMatch(key) == true)
            {
                keyValueList.Add(new KeyValue(key, value));
            }

            iterator.Next();
        }

        await Task.CompletedTask;
        return [.. keyValueList];
    }

    public async Task DeleteAsync(string key, CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);

        var writeOptions = new WriteOptions();
        var columnFamilyHandle = _columnFamilyHandle;
        _db.Remove(key, columnFamilyHandle, writeOptions);
        await Task.CompletedTask;
    }

    public async Task<string[]> DeleteManyAsync(string[] keys, CancellationToken cancellationToken)
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);

        var removedKeyList = new List<string>(keys.Length);
        var writeOptions = new WriteOptions();
        var columnFamilyHandle = _columnFamilyHandle;
        foreach (var key in keys)
        {
            if (_db.HasKey(key, columnFamilyHandle) == true)
            {
                _db.Remove(key, columnFamilyHandle, writeOptions);
                removedKeyList.Add(key);
            }
        }

        await Task.CompletedTask;
        return [.. removedKeyList];
    }
}
