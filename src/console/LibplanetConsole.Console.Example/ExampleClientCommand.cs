using JSSoft.Commands;

namespace LibplanetConsole.Console.Example;

[CommandSummary("Example client commands for a quick start.")]
internal sealed class ExampleClientCommand(IApplication application) : CommandMethodBase
{
    [CommandMethod]
    public void Subscribe(string clientAddress)
    {
        var client = application.GetClient(clientAddress);

        if (client.GetService(typeof(IExampleClient)) is IExampleClient sampleClient)
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

        if (client.GetService(typeof(IExampleClient)) is IExampleClient sampleClient)
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
