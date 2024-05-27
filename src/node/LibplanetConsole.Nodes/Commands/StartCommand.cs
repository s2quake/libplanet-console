using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Nodes.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Start node.")]
[method: ImportingConstructor]
internal sealed class StartCommand(ApplicationBase application, Node node) : CommandAsyncBase
{
    public override bool IsEnabled => node.IsRunning is false;

    [CommandProperty("seed-endpoint", 's', DefaultValue = "")]
    public string SeedEndPoint { get; set; } = string.Empty;

    protected override async Task OnExecuteAsync(
        CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
    {
        var seedEndPoint = application.Info.NodeEndPoint;
        var nodeOptions = seedEndPoint != string.Empty
            ? await NodeOptions.CreateAsync(seedEndPoint, cancellationToken)
            : new NodeOptions
            {
                GenesisOptions = application.DefaultGenesisOptions,
            };
        await node.StartAsync(nodeOptions, cancellationToken);
    }
}
