using JSSoft.Commands;
using JSSoft.Terminals;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Actions;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Console.Commands;

[CommandSummary("Provides node-related commands")]
public sealed partial class NodeCommand(
    IServiceProvider serviceProvider, INodeCollection nodes, IAddressCollection addresses)
    : NodeCommandMethodBase(serviceProvider), IExecutable
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

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
        var node = GetNodeOrCurrent(NodeAddress);
        var nodeInfo = InfoUtility.GetInfo(_serviceProvider, obj: node);
        Out.WriteLineAsJson(nodeInfo);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    [CommandSummary("Attaches to the node which is already running")]
    public async Task AttachAsync(CancellationToken cancellationToken = default)
    {
        var node = GetNodeOrCurrent(NodeAddress);
        await node.AttachAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    [CommandSummary("Detaches from the node")]
    public async Task DetachAsync(CancellationToken cancellationToken = default)
    {
        var node = GetNodeOrCurrent(NodeAddress);
        await node.DetachAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    [CommandSummary("Starts a node of the specified address")]
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        var node = GetNodeOrCurrent(NodeAddress);
        await node.StartAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    [CommandSummary("Stops a node of the specified address")]
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        var node = GetNodeOrCurrent(NodeAddress);
        await node.StopAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Gets or sets the current node")]
    public void Current(
        [CommandSummary("The address of the node")]
        [CommandParameterCompletion(nameof(GetNodeAddresses))]
        Address nodeAddress = default)
    {
        var node = GetNodeOrDefault(nodeAddress);
        if (node is not null)
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
        var node = GetNodeOrCurrent(NodeAddress);
        var action = new StringAction { Value = text };
        await node.SendTransactionAsync([action], cancellationToken);
        await Out.WriteLineAsync($"{node.Address.ToShortString()}: {text}");
    }

    [CommandMethod("command")]
    [CommandMethodProperty(nameof(NodeAddress))]
    [CommandSummary("Gets the command line to start a node")]
    public void GetCommandLine()
    {
        var node = GetNodeOrCurrent(NodeAddress);
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
}
