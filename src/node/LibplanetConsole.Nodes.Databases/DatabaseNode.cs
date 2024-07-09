using System.ComponentModel.Composition;
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
    private TableCollection? _tables;

    public event EventHandler? Started;

    public event EventHandler? Stopped;

    public DatabaseInfo Info { get; private set; }

    public bool IsRunning => _db is not null;

    public TableCollection Tables
        => _tables ?? throw new InvalidOperationException("The database is not running.");

    ITableCollection IDatabaseNode.Tables => Tables;

    public async Task StartAsync(
        DatabaseOptions options, CancellationToken cancellationToken)
    {
        if (_db is not null)
        {
            throw new InvalidOperationException("The explorer is already running.");
        }

        var dbOptions = new DbOptions()
            .SetCreateIfMissing(true);
        var columnFamilyOptions = new ColumnFamilyOptions()
            .SetCreateMissingColumnFamilies(true);
        var path = options.DatabasePath;
        var columnFamilies = GetColumnFamilies(dbOptions, columnFamilyOptions, path);
        _db = RocksDb.Open(dbOptions, options.DatabasePath, columnFamilies);
        _tables = new(_db, columnFamilyOptions, [.. columnFamilies.Select(item => item.Name)]);
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
        _tables?.Dispose();
        _tables = null;
        _db.Dispose();
        _db = null;
        Info = new() { };
        logger.Debug("Database is stopped.");
        Stopped?.Invoke(this, EventArgs.Empty);
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

    private static ColumnFamilies GetColumnFamilies(
        DbOptions dbOptions, ColumnFamilyOptions columnFamilyOptions, string path)
    {
        var columnFamilies = new ColumnFamilies(columnFamilyOptions);
        if (RocksDb.TryListColumnFamilies(dbOptions, path, out var items) == true)
        {
            for (var i = 0; i < items.Length; i++)
            {
                columnFamilies.Add(items[i], columnFamilyOptions);
            }
        }

        return columnFamilies;
    }
}
