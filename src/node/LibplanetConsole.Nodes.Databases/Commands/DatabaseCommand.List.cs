using JSSoft.Commands;
using JSSoft.Commands.Extensions;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Nodes.Databases.Commands;

internal sealed partial class DatabaseCommand
{
    [CommandMethod(Aliases = ["ls"])]
    [CommandMethodStaticProperty(typeof(ListProperties))]
    public async Task ListAsync(
        string find = "", CancellationToken cancellationToken = default)
    {
        var databaseNode = serviceProvider.GetService<IDatabaseNode>();
        var prefix = ListProperties.Prefix;
        var keyValues = await databaseNode.SeekManyAsync(find, prefix, cancellationToken);
        using var sw = new StringWriter();
        var tableDataBuilder = new TableDataBuilder(3);

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
