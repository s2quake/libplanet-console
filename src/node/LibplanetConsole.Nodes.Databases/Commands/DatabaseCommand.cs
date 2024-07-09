using System.ComponentModel.Composition;
using System.Text;
using JSSoft.Commands;
using JSSoft.Commands.Extensions;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Databases;

namespace LibplanetConsole.Nodes.Databases.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Provides commands for the database(RocksDb).")]
[method: ImportingConstructor]
internal sealed partial class DatabaseCommand(IServiceProvider serviceProvider)
    : CommandMethodBase(aliases: ["db"])
{
    [CommandMethod]
    public async Task StartAsync(
        string databaePath = "", CancellationToken cancellationToken = default)
    {
        var databaseNode = serviceProvider.GetService<IDatabaseNode>();
        var databaseOptions = new DatabaseOptions
        {
            DatabasePath = databaePath,
        };
        await databaseNode.StartAsync(databaseOptions, cancellationToken);
    }

    [CommandMethod]
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        var databaseNode = serviceProvider.GetService<IDatabaseNode>();
        await databaseNode.StopAsync(cancellationToken);
    }

    [CommandMethod]
    public async Task PutAsync(
        string key, string value, CancellationToken cancellationToken = default)
    {
        var databaseNode = serviceProvider.GetService<IDatabaseNode>();
        await databaseNode.PutAsync(key, value, cancellationToken);
    }

    [CommandMethod]
    public async Task GetAsync(
        string key, CancellationToken cancellationToken = default)
    {
        var databaseNode = serviceProvider.GetService<IDatabaseNode>();
        var value = await databaseNode.SeekAsync(key, cancellationToken);
        await Out.WriteLineAsync(value);
    }

    [CommandMethod(Aliases = ["rm"])]
    public async Task DeleteAsync(
        string key, CancellationToken cancellationToken = default)
    {
        var databaseNode = serviceProvider.GetService<IDatabaseNode>();
        await databaseNode.DeleteAsync(key, cancellationToken);
    }

    [CommandMethod(Aliases = ["init"])]
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var databaseNode = serviceProvider.GetService<IDatabaseNode>();
        var items = Shuffle(GetWords());

        var keyValues = new KeyValue[items.Length];
        for (var i = 0; i < items.Length; i++)
        {
            var key = items[i];
            var value = $"{Guid.NewGuid()}";
            keyValues[i] = new KeyValue(key, value);
        }

        await databaseNode.PutRangeAsync(keyValues, cancellationToken);
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
