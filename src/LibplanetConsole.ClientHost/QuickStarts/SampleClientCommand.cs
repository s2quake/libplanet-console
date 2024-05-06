using System.ComponentModel.Composition;
using JSSoft.Commands;

namespace LibplanetConsole.ClientHost.QuickStarts;

[Export(typeof(ICommand))]
[CommandSummary("Sample client commands for a quick start.")]
[method: ImportingConstructor]
internal sealed class SampleClientCommand(ISampleClient sampleClient)
    : CommandMethodBase
{
    [CommandMethod]
    public void Subscribe()
    {
        sampleClient.Subscribe();
    }

    [CommandMethod]
    public void Unsubscribe()
    {
        sampleClient.Unsubscribe();
    }
}
