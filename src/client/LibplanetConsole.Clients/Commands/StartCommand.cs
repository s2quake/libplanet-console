using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Clients.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Start client.")]
[method: ImportingConstructor]
internal sealed class StartCommand(IClient client) : CommandAsyncBase
{
    public override bool IsEnabled => client.IsRunning is false;

    [CommandPropertyRequired]
    [CommandSummary("Indicates the EndPoint of the node to connect to.")]
    public string NodeEndPoint { get; set; } = string.Empty;

    [CommandProperty("seed")]
    [CommandSummary("Use --node-end-point as the Seed EndPoint. " +
                    "Get the EndPoint of the Node to connect to from Seed.")]
    public bool IsSeed { get; init; }

    protected override async Task OnExecuteAsync(
        CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
    {
        var seedEndPoint = NodeEndPoint;
        var clientOptions = new ClientOptions
        {
            NodeEndPoint = await SeedUtility.GetNodeEndPointAsync(seedEndPoint, cancellationToken),
        };
        await client.StartAsync(clientOptions, cancellationToken);
        await Out.WriteLineAsJsonAsync(client.Info);
    }
}
