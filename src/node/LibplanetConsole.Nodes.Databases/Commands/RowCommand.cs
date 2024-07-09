using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Databases;

namespace LibplanetConsole.Nodes.Databases.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Provides commands for the row(RocksDb).")]
[method: ImportingConstructor]
internal sealed partial class RowCommand(IServiceProvider serviceProvider)
    : CommandMethodBase()
{
    [CommandMethod]
    [CommandMethodStaticProperty(typeof(TableProperties))]
    public async Task PutAsync(
        string key, string value, CancellationToken cancellationToken = default)
    {
        var databaseNode = serviceProvider.GetService<IDatabaseNode>();
        var table = databaseNode.Tables[TableProperties.Name];
        var options = new PutOptions
        {
            Key = key,
            Value = value,
        };
        await table.PutAsync(options, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(TableProperties))]
    public async Task GetAsync(
        string key, CancellationToken cancellationToken = default)
    {
        var databaseNode = serviceProvider.GetService<IDatabaseNode>();
        var table = databaseNode.Tables[TableProperties.Name];
        var options = new SeekOptions
        {
            Key = key,
        };
        var value = await table.SeekAsync(options, cancellationToken);
        await Out.WriteLineAsync(value);
    }

    [CommandMethod(Aliases = ["rm"])]
    public async Task DeleteAsync(
        string key, CancellationToken cancellationToken = default)
    {
        var databaseNode = serviceProvider.GetService<IDatabaseNode>();
        var table = databaseNode.Tables[TableProperties.Name];
        await table.DeleteAsync(key, cancellationToken);
    }

    private static class TableProperties
    {
        [CommandProperty("table", InitValue = "default")]
        public static string Name { get; set; } = string.Empty;
    }
}
