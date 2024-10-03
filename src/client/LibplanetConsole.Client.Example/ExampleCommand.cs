using JSSoft.Commands;

namespace LibplanetConsole.Client.Example;

[CommandSummary("Example client commands for a quick start.")]
internal sealed class ExampleCommand(IExample sampleClient)
    : CommandMethodBase
{
    [CommandMethod]
    public void Subscribe() => sampleClient.Subscribe();

    [CommandMethod]
    public void Unsubscribe() => sampleClient.Unsubscribe();
}
