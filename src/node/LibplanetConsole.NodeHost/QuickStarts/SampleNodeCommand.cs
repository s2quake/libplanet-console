using System.ComponentModel.Composition;
using System.Text;
using JSSoft.Commands;
using LibplanetConsole.Common;

namespace LibplanetConsole.NodeHost.QuickStarts;

[Export(typeof(ICommand))]
[CommandSummary("Sample node commands for a quick start.")]
[method: ImportingConstructor]
internal sealed class SampleNodeCommand(ISampleNodeContent sampleNode)
    : CommandMethodBase
{
    [CommandMethod]
    public void Subscribe(string address)
    {
        sampleNode.Subscribe(AddressUtility.Parse(address));
    }

    [CommandMethod]
    public void Unsubscribe(string address)
    {
        sampleNode.Unsubscribe(AddressUtility.Parse(address));
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
            sb.AppendLine(AddressUtility.ToString(address));
        }

        Out.WriteLine(sb.ToString());
    }
}
