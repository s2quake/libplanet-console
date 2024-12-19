using JSSoft.Commands;
using JSSoft.Terminals;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Actions;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Console.Extensions;

namespace LibplanetConsole.Console.Commands;

[CommandSummary("Provides node-related commands")]
public sealed partial class NodeCommand(
    IServiceProvider serviceProvider, INodeCollection nodes, IAddressCollection addresses)
    : CommandMethodBase, IExecutable
{
    [CommandProperty("node", 'N', InitValue = "")]
    [CommandSummary("Specifies the address of the node to use")]
    [CommandPropertyCompletion(nameof(GetNodeAddresses))]
    public static string NodeAddress { get; set; } = string.Empty;

    [CommandMethod]
    [CommandSummary("Displays the list of nodes")]
    public void List()
    {
        var tsb = new TerminalStringBuilder();
        for (var i = 0; i < nodes.Count; i++)
        {
            var node = nodes[i];
            var address = node.Address;
            var isCurrent = nodes.Current == node;
            tsb.Foreground = GetForeground(node: node, isCurrent);
            tsb.IsBold = node.IsRunning == true;
            if (addresses.TryGetAlias(address, out var alias) is true)
            {
                tsb.AppendLine($"{node} ({alias})");
            }
            else
            {
                tsb.AppendLine($"{node}");
            }

            tsb.ResetOptions();
            tsb.Append(string.Empty);
        }

        Out.Write(tsb.ToString());
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    [CommandSummary("Displays the node information of the specified address")]
    public void Info()
    {
        var nodeAddress = addresses.ToAddress(NodeAddress);
        var node = nodes.GetNodeOrCurrent(nodeAddress);
        var nodeInfo = InfoUtility.GetInfo(serviceProvider, obj: node);
        Out.WriteLineAsJson(nodeInfo);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    [CommandSummary("Attaches the node instance to the node process")]
    public async Task AttachAsync(CancellationToken cancellationToken = default)
    {
        var nodeAddress = addresses.ToAddress(NodeAddress);
        var node = nodes.GetNodeOrCurrent(nodeAddress);
        await node.AttachAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    [CommandSummary("Detaches the node instance from the node process")]
    public async Task DetachAsync(CancellationToken cancellationToken = default)
    {
        var nodeAddress = addresses.ToAddress(NodeAddress);
        var node = nodes.GetNodeOrCurrent(nodeAddress);
        await node.DetachAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    [CommandSummary("Starts the node")]
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        var nodeAddress = addresses.ToAddress(NodeAddress);
        var node = nodes.GetNodeOrCurrent(nodeAddress);
        await node.StartAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    [CommandSummary("Stops the node")]
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        var nodeAddress = addresses.ToAddress(NodeAddress);
        var node = nodes.GetNodeOrCurrent(nodeAddress);
        await node.StopAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Gets or sets the current node")]
    public void Current(
        [CommandSummary("The address of the node")]
        string nodeAddress = "")
    {
        var address = addresses.ToAddress(nodeAddress);
        if (address != default && nodes.GetNodeOrCurrent(address) is { } node)
        {
            nodes.Current = node;
        }

        if (nodes.Current is not null)
        {
            Out.WriteLine(nodes.Current);
        }
        else
        {
            Out.WriteLine("No node is selected.");
        }
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    [CommandSummary("Sends a transaction using a string")]
    public async Task TxAsync(
        [CommandSummary("Specifies the text to send")]
        string text,
        CancellationToken cancellationToken)
    {
        var nodeAddress = addresses.ToAddress(NodeAddress);
        var node = nodes.GetNodeOrCurrent(nodeAddress);
        var action = new StringAction { Value = text };
        await node.SendTransactionAsync([action], cancellationToken);
        await Out.WriteLineAsync($"{node.Address.ToShortString()}: {text}");
    }

    [CommandMethod("command")]
    [CommandMethodProperty(nameof(NodeAddress))]
    [CommandSummary("Gets the command line to start a node")]
    public void GetCommandLine()
    {
        var nodeAddress = addresses.ToAddress(NodeAddress);
        var node = nodes.GetNodeOrCurrent(nodeAddress);
        Out.WriteLine(node.GetCommandLine());
    }

    void IExecutable.Execute()
    {
        if (Context.HelpCommand is HelpCommandBase helpCommand)
        {
            helpCommand.PrintHelp(this);
        }
    }

    private static TerminalColorType? GetForeground(INode node, bool isCurrent)
    {
        if (node.IsRunning == true)
        {
            return isCurrent == true ? TerminalColorType.BrightGreen : null;
        }

        return TerminalColorType.BrightBlack;
    }

    private string[] GetNodeAddresses() =>
    [
        .. nodes.Where(item => item.Alias != string.Empty).Select(item => item.Alias),
        .. nodes.Select(node => $"{node.Address}"),
    ];
}
