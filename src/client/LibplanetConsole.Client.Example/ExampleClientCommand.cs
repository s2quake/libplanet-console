using System.ComponentModel.Composition;
using JSSoft.Commands;

namespace LibplanetConsole.Clients.Examples;

[Export(typeof(ICommand))]
[CommandSummary("Example client commands for a quick start.")]
[method: ImportingConstructor]
internal sealed class ExampleClientCommand(IExampleClient sampleClient)
    : CommandMethodBase
{
    [CommandMethod]
    public void Subscribe() => sampleClient.Subscribe();

    [CommandMethod]
    public void Unsubscribe() => sampleClient.Unsubscribe();
}
