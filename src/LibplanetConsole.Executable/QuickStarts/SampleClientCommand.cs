using System.ComponentModel.Composition;
using JSSoft.Commands;

namespace LibplanetConsole.Executable.QuickStarts;

[Export(typeof(ICommand))]
[CommandSummary("Sample client commands for a quick start.")]
[method: ImportingConstructor]
internal sealed class SampleClientCommand(IApplication application) : CommandMethodBase
{
    [CommandMethod]
    public void Subscribe(string clientAddress)
    {
        var client = application.GetClient(clientAddress);

        if (client.GetService(typeof(ISampleClient)) is ISampleClient sampleClient)
        {
            sampleClient.Subscribe();
        }
        else
        {
            throw new InvalidOperationException(
                "The client does not support the sample client service.");
        }
    }

    [CommandMethod]
    public void Unsubscribe(string clientAddress)
    {
        var client = application.GetClient(clientAddress);

        if (client.GetService(typeof(ISampleClient)) is ISampleClient sampleClient)
        {
            sampleClient.Unsubscribe();
        }
        else
        {
            throw new InvalidOperationException(
                "The client does not support the sample client service.");
        }
    }
}
