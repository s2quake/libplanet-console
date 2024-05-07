using System.ComponentModel.Composition;
using System.Text;
using JSSoft.Commands;

namespace LibplanetConsole.Executable.QuickStarts;

[Export(typeof(ICommand))]
[CommandSummary("Sample node commands for a quick start.")]
[method: ImportingConstructor]
internal sealed class SampleNodeCommand(IApplication application) : CommandMethodBase
{
    [CommandMethod]
    public void Subscribe(string nodeAddress, string clientAddress)
    {
        var node = application.GetNode(nodeAddress);
        var client = application.GetClient(clientAddress);

        if (node.GetService(typeof(ISampleNode)) is ISampleNode sampleNode)
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

        if (node.GetService(typeof(ISampleNode)) is ISampleNode sampleNode)
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

        if (node.GetService(typeof(ISampleNode)) is ISampleNode sampleNode)
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

        if (node.GetService(typeof(ISampleNode)) is ISampleNode sampleNode)
        {
            var addresses = await sampleNode.GetAddressesAsync(cancellationToken);
            var sb = new StringBuilder();
            foreach (var address in addresses)
            {
                sb.AppendLine(address.ToString());
            }

            Out.Write(sb.ToString());
        }
        else
        {
            throw new InvalidOperationException(
                "The node does not support the sample node service.");
        }
    }
}
