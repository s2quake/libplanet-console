using JSSoft.Commands;
using LibplanetConsole.Common;

namespace LibplanetConsole.Client.Commands;

[CommandSummary("Starts the client")]
internal sealed class StartCommand(Client client) : CommandAsyncBase
{
    public override bool IsEnabled => client.IsRunning is false;

    [CommandPropertyRequired(DefaultValue = "")]
    [CommandSummary("Specifies the hub url to connect")]
    public string HubUrl { get; set; } = string.Empty;

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var nodeUrl = UriUtility.ParseOrFallback(HubUrl, client.NodeUrl);
        client.NodeUrl = nodeUrl;
        await client.StartAsync(cancellationToken);
    }
}
