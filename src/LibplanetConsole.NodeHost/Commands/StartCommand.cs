using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.NodeHost.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Start node.")]
[method: ImportingConstructor]
internal sealed class StartCommand(INode node) : CommandAsyncBase
{
    public override bool IsEnabled => node.IsRunning is false;

    [CommandProperty("seed-endpoint", 's', DefaultValue = "")]
    public string SeedEndPoint { get; set; } = string.Empty;

    protected override async Task OnExecuteAsync(
        CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
    {
        var seedEndPoint = SeedEndPoint;
        var nodeOptions = seedEndPoint != string.Empty
            ? await NodeOptionsUtility.GetNodeOptionsAsync(seedEndPoint, cancellationToken)
            : NodeOptionsUtility.GetNodeOptions(node);
        await node.StartAsync(nodeOptions, cancellationToken);
        await Out.WriteLineAsJsonAsync(node.Info);
    }
}
