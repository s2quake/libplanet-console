using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common;

namespace LibplanetConsole.Clients.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Start client.")]
[method: ImportingConstructor]
internal sealed class StartCommand(Client client) : CommandAsyncBase
{
    public override bool IsEnabled => client.IsRunning is false;

    [CommandPropertyRequired(DefaultValue = "")]
    [CommandSummary("Indicates the EndPoint of the node to connect to.")]
    public string NodeEndPoint { get; set; } = string.Empty;

    [CommandProperty("seed")]
    [CommandSummary("Use --node-end-point as the Seed EndPoint. " +
                    "Get the EndPoint of the Node to connect to from Seed.")]
    public bool IsSeed { get; init; }

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var isSeed = IsSeed;
        var nodeEndPoint = AppEndPoint.ParseOrFallback(NodeEndPoint, client.NodeEndPoint);
        client.NodeEndPoint = await SeedUtility.GetNodeEndPointAsync(
            nodeEndPoint, isSeed, cancellationToken);
        await client.StartAsync(cancellationToken);
    }
}
