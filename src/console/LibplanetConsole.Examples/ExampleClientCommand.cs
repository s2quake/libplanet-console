using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Consoles;

namespace LibplanetConsole.Examples;

[Export(typeof(ICommand))]
[CommandSummary("Example client commands for a quick start.")]
[method: ImportingConstructor]
internal sealed class ExampleClientCommand(IApplication application) : CommandMethodBase
{
    [CommandMethod]
    public void Subscribe(string clientAddress)
    {
        var client = application.GetClient(clientAddress);

        if (client.GetService(typeof(IExampleClientContent)) is IExampleClientContent sampleClient)
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

        if (client.GetService(typeof(IExampleClientContent)) is IExampleClientContent sampleClient)
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
