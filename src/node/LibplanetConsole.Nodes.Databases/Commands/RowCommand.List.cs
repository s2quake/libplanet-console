using JSSoft.Commands;
using JSSoft.Commands.Extensions;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Nodes.Databases.Commands;

internal sealed partial class RowCommand
{
    [CommandMethod(Aliases = ["ls"])]
    [CommandMethodStaticProperty(typeof(TableProperties))]
    [CommandMethodStaticProperty(typeof(ListProperties))]
    public async Task ListAsync(
        string find = "", CancellationToken cancellationToken = default)
    {
        var databaseNode = serviceProvider.GetService<IDatabaseNode>();
        var table = databaseNode.Tables[TableProperties.Name];
        var prefix = ListProperties.Prefix;
        var keyValues = await table.SeekManyAsync(find, prefix, cancellationToken);
        using var sw = new StringWriter();
        var tableDataBuilder = new TableDataBuilder(["index", "key", "value"]);

        for (var i = 0; i < keyValues.Length; i++)
        {
            var key = keyValues[i].Key;
            var value = keyValues[i].Value;

            tableDataBuilder.Add([i, key, value]);
        }

        sw.PrintTableData(tableDataBuilder.Data, hasHeader: false);
        await Out.WriteLineAsync(sw.ToString());
    }

    private static class ListProperties
    {
        [CommandProperty]
        public static string Prefix { get; set; } = string.Empty;
    }
}
