using System.ComponentModel.Composition;
using DotNet.Globbing;
using LibplanetConsole.Databases;
using LibplanetConsole.Databases.Serializations;
using LibplanetConsole.Frameworks;
using RocksDbSharp;
using Serilog;

namespace LibplanetConsole.Nodes.Databases;

[Export(typeof(IDatabaseNode))]
[Export(typeof(IApplicationService))]
[Export]
[method: ImportingConstructor]
internal sealed class DatabaseNode(ILogger logger) : IDatabaseNode, IApplicationService
{
    private RocksDb? _db;

    public event EventHandler? Started;

    public event EventHandler? Stopped;

    public DatabaseInfo Info { get; private set; }

    public bool IsRunning => _db is not null;

    public async Task StartAsync(
        DatabaseOptions options, CancellationToken cancellationToken)
    {
        if (_db is not null)
        {
            throw new InvalidOperationException("The explorer is already running.");
        }

        DbOptions dbOptions = new DbOptions().SetCreateIfMissing(true);
        _db = RocksDb.Open(dbOptions, options.DatabasePath);
        await Task.CompletedTask;
        Info = new() { IsRunning = true, };
        logger.Debug("Database is started: {EndPoint}", Info.EndPoint);
        Started?.Invoke(this, EventArgs.Empty);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_db is null)
        {
            throw new InvalidOperationException("The database is not running.");
        }

        await Task.CompletedTask;
        _db.Dispose();
        _db = null;
        Info = new() { };
        logger.Debug("Database is stopped.");
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    public async Task PutAsync(string key, string value, CancellationToken cancellationToken)
    {
        if (_db is null)
        {
            throw new InvalidOperationException("The database is not running.");
        }

        var writeOptions = new WriteOptions();
        _db.Put(key, value, writeOptions: writeOptions);
        await Task.CompletedTask;
    }

    public async Task PutRangeAsync(KeyValue[] keyValues, CancellationToken cancellationToken)
    {
        if (_db is null)
        {
            throw new InvalidOperationException("The database is not running.");
        }

        var writeOptions = new WriteOptions();
        var writeBatch = new WriteBatch();
        for (var i = 0; i < keyValues.Length; i++)
        {
            var valuePair = keyValues[i];
            writeBatch.Put(valuePair.Key, valuePair.Value);
        }

        _db.Write(writeBatch, writeOptions);
        await Task.CompletedTask;
    }

    public async Task<string> SeekAsync(string key, CancellationToken cancellationToken)
    {
        if (_db is null)
        {
            throw new InvalidOperationException("The database is not running.");
        }

        var readOptions = new ReadOptions();
        var value = _db.Get(key, readOptions: readOptions);
        await Task.CompletedTask;
        return value;
    }

    public async Task<KeyValue[]> SeekManyAsync(
        string find, string prefix, CancellationToken cancellationToken)
    {
        if (_db is null)
        {
            throw new InvalidOperationException("The database is not running.");
        }

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
        if (_db is null)
        {
            throw new InvalidOperationException("The database is not running.");
        }

        var writeOptions = new WriteOptions();
        _db.Remove(key, writeOptions: writeOptions);
        await Task.CompletedTask;
    }

    async Task IApplicationService.InitializeAsync(
        IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var databasePath = ApplicationSettingsParser.Peek<DatabaseNodeSettings>().DatabasePath;
        if (databasePath != string.Empty)
        {
            var options = new DatabaseOptions
            {
                DatabasePath = databasePath,
            };
            await StartAsync(options, cancellationToken);
        }
    }
}
