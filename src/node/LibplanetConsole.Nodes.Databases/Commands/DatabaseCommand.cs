using System.ComponentModel.Composition;
using System.Text;
using DotNet.Globbing;
using JSSoft.Commands;
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
}
