using System.ComponentModel.Composition;
using System.Text;
using DotNet.Globbing;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Databases;

namespace LibplanetConsole.Nodes.Databases.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Provides commands for the table(RocksDb).")]
[method: ImportingConstructor]
internal sealed partial class TableCommand(IServiceProvider serviceProvider)
    : CommandMethodBase()
{
    [CommandMethod(Aliases = ["ls"])]
    public void List(string filter = "")
    {
        var databaseNode = serviceProvider.GetService<IDatabaseNode>();
        var glob = Glob.Parse(filter == string.Empty ? "*" : filter);
        var query = from table in databaseNode.Tables
                    where glob.IsMatch(table.Name) == true
                    select table.Name;
        var sb = new StringBuilder();
        sb.AppendLines(query);
        Out.Write(sb.ToString());
    }

    [CommandMethod]
    public async Task NewAsync(string tableName, CancellationToken cancellationToken)
    {
        var databaseNode = serviceProvider.GetService<IDatabaseNode>();
        var tables = databaseNode.Tables;
        await tables.CreateAsync(tableName, cancellationToken);
        await Out.WriteLineAsync($"Table '{tableName}' is created.");
    }

    [CommandMethod(Aliases = ["rm"])]
    public async Task DeleteAsync(string tableName, CancellationToken cancellationToken)
    {
        var databaseNode = serviceProvider.GetService<IDatabaseNode>();
        var tables = databaseNode.Tables;
        await tables.DeleteAsync(tableName, cancellationToken);
        await Out.WriteLineAsync($"Table '{tableName}' is deleted.");
    }

    [CommandMethod]
    public async Task FillAsync(string tableName, CancellationToken cancellationToken)
    {
        var databaseNode = serviceProvider.GetService<IDatabaseNode>();
        var table = databaseNode.Tables[tableName];
        var items = Shuffle(GetWords());
        var keyValues = new KeyValue[items.Length];
        for (var i = 0; i < items.Length; i++)
        {
            var key = items[i];
            var value = $"{Guid.NewGuid()}";
            keyValues[i] = new KeyValue(key, value);
        }

        var options = new PutRangeOptions
        {
            KeyValues = keyValues,
        };
        await table.PutRangeAsync(options, cancellationToken);
        await Out.WriteLineAsync($"Table '{tableName}' has been filled with random items.");
    }

    [CommandMethod]
    public async Task ResetAsync(string tableName, CancellationToken cancellationToken)
    {
        var databaseNode = serviceProvider.GetService<IDatabaseNode>();
        var tables = databaseNode.Tables;
        await tables.DeleteAsync(tableName, cancellationToken);
        await tables.CreateAsync(tableName, cancellationToken);
        await Out.WriteLineAsync($"Table '{tableName}' has been reset.");
    }

    private static string[] GetWords()
    {
        var assembly = typeof(DatabaseNode).Assembly;
        var ns = typeof(DatabaseNode).Namespace;
        var resourceName = string.Join(".", ns, "Resources", "words.txt");
        var resourceStream = assembly.GetManifestResourceStream(resourceName)!;
        using var stream = new StreamReader(resourceStream);
        var text = stream.ReadToEnd();
        int i = 0;
        using var sr1 = new StringReader(text);
        while (sr1.ReadLine() is { } line1)
        {
            if (line1 != string.Empty)
            {
                i++;
            }
        }

        var words = new string[i];
        i = 0;
        using var sr2 = new StringReader(text);
        while (sr2.ReadLine() is { } line2)
        {
            if (line2 != string.Empty)
            {
                words[i++] = line2;
            }
        }

        return words;
    }

    private static string[] Shuffle(string[] items)
    {
        var itemList1 = items.ToList();
        var itemList2 = new List<string>(itemList1.Count);
        while (itemList1.Count > 0)
        {
            var index = Random.Shared.Next(itemList1.Count);
            var item = itemList1[index];
            itemList1.RemoveAt(index);
            itemList2.Add(item);
        }

        return [.. itemList2];
    }
}
