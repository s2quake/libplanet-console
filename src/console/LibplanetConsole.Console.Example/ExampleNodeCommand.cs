using System.Text;
using JSSoft.Commands;

namespace LibplanetConsole.Console.Example;

[CommandSummary("Example node commands for a quick start.")]
internal sealed class ExampleNodeCommand(IApplication application) : CommandMethodBase
{
    [CommandMethod]
    public void Subscribe(string nodeAddress, string clientAddress)
    {
        var node = application.GetNode(nodeAddress);
        var client = application.GetClient(clientAddress);

        if (node.GetService(typeof(IExampleNode)) is IExampleNode sampleNode)
        {
            sampleNode.Subscribe(client.Address);
        }
        else
        {
            throw new InvalidOperationException(
                "The node does not support the sample node service.");
        }
    }

    [CommandMethod]
    public void Unsubscribe(string nodeAddress, string clientAddress)
    {
        var node = application.GetNode(nodeAddress);
        var client = application.GetClient(clientAddress);

        if (node.GetService(typeof(IExampleNode)) is IExampleNode sampleNode)
        {
            sampleNode.Unsubscribe(client.Address);
        }
        else
        {
            throw new InvalidOperationException(
                "The node does not support the sample node service.");
        }
    }

    [CommandMethod]
    public void Count(string nodeAddress)
    {
        var node = application.GetNode(nodeAddress);

        if (node.GetService(typeof(IExampleNode)) is IExampleNode sampleNode)
        {
            Out.WriteLine(sampleNode.Count);
        }
        else
        {
            throw new InvalidOperationException(
                "The node does not support the sample node service.");
        }
    }

    [CommandMethod]
    public async Task ListAsync(string nodeAddress, CancellationToken cancellationToken)
    {
        var node = application.GetNode(nodeAddress);

        if (node.GetService(typeof(IExampleNode)) is IExampleNode sampleNode)
        {
            var addresses = await sampleNode.GetAddressesAsync(cancellationToken);
            var sb = new StringBuilder();
            foreach (var address in addresses)
            {
                sb.AppendLine(address.ToString());
            }

            await Out.WriteAsync(sb.ToString());
        }
        else
        {
            throw new InvalidOperationException(
                "The node does not support the sample node service.");
        }
    }
}
