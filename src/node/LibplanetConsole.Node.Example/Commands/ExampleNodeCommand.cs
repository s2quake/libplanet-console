using System.Text;
using JSSoft.Commands;

namespace LibplanetConsole.Node.Example.Commands;

[CommandSummary("Example node commands for a quick start.")]
internal sealed class ExampleNodeCommand(IExampleNode sampleNode)
    : CommandMethodBase
{
    [CommandMethod]
    public void Subscribe(string address)
    {
        sampleNode.Subscribe(new Address(address));
    }

    [CommandMethod]
    public void Unsubscribe(string address)
    {
        sampleNode.Unsubscribe(new Address(address));
    }

    [CommandMethod]
    public void Count()
    {
        Out.WriteLine(sampleNode.Count);
    }

    [CommandMethod]
    public async Task ListAsync(CancellationToken cancellationToken)
    {
        var addresses = await sampleNode.GetAddressesAsync(cancellationToken);
        var sb = new StringBuilder();
        foreach (var address in addresses)
        {
            sb.AppendLine(address.ToString());
        }

        await Out.WriteLineAsync(sb.ToString());
    }
}
