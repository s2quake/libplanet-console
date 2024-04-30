using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.ClientServices;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.ClientHost.Commands;

[Export(typeof(ICommand))]
[method: ImportingConstructor]
internal sealed class StartCommand(IClient client) : CommandAsyncBase
{
    public override bool IsEnabled => client.IsRunning is false;

    [CommandPropertyRequired]
    public string SeedEndPoint { get; set; } = string.Empty;

    protected override async Task OnExecuteAsync(
        CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
    {
        var seedEndPoint = SeedEndPoint;
        var clientOptions = new ClientOptions
        {
            NodeEndPoint = await SeedUtility.GetNodeEndPointAsync(seedEndPoint, cancellationToken),
        };
        await client.StartAsync(clientOptions, cancellationToken);
        await Out.WriteLineAsJsonAsync(client.Info);
    }
}
