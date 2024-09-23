using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common;

namespace LibplanetConsole.Client.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Start client.")]
[method: ImportingConstructor]
internal sealed class StartCommand(Client client) : CommandAsyncBase
{
    public override bool IsEnabled => client.IsRunning is false;

    [CommandPropertyRequired(DefaultValue = "")]
    [CommandSummary("Indicates the EndPoint of the node to connect to.")]
    public string NodeEndPoint { get; set; } = string.Empty;

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var nodeEndPoint = AppEndPoint.ParseOrFallback(NodeEndPoint, client.NodeEndPoint);
        client.NodeEndPoint = nodeEndPoint;
        await client.StartAsync(cancellationToken);
    }
}
