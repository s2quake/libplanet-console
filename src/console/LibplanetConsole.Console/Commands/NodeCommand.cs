using JSSoft.Commands;
using JSSoft.Terminals;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Actions;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Console.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Commands;

[CommandSummary("Provides node-related commands")]
public sealed partial class NodeCommand(IServiceProvider serviceProvider, INodeCollection nodes)
    : CommandMethodBase, IExecutable
{
    [CommandPropertyRequired(DefaultValue = "")]
    [CommandSummary("Specifies the address of the client")]
    [CommandPropertyCompletion(nameof(GetNodeAddresses))]
    public static string Address { get; set; } = string.Empty;

    [CommandPropertySwitch("detail")]
    [CommandSummary("If set, detailed information is displayed")]
    public static bool IsDetailed { get; set; }

    [CommandMethod]
    [CommandMethodProperty(nameof(IsDetailed))]
    [CommandSummary("Displays the list of nodes")]
    public void List()
    {
        GetListAction(IsDetailed).Invoke();

        Action GetListAction(bool isDetailed) => isDetailed switch
        {
            false => ListNormal,
            true => ListDetailed,
        };
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Displays the node information of the specified address")]
    public void Info()
    {
        var address = Address;
        var node = nodes.GetNodeOrCurrent(address);
        var nodeInfo = InfoUtility.GetInfo(serviceProvider, obj: node);
        Out.WriteLineAsJson(nodeInfo);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Attaches to the node which is already running")]
    public async Task AttachAsync(CancellationToken cancellationToken = default)
    {
        var address = Address;
        var node = nodes.GetNodeOrCurrent(address);
        await node.AttachAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Detaches from the node")]
    public async Task DetachAsync(CancellationToken cancellationToken = default)
    {
        var address = Address;
        var node = nodes.GetNodeOrCurrent(address);
        await node.DetachAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Starts a node of the specified address")]
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        var address = Address;
        var node = nodes.GetNodeOrCurrent(address);
        await node.StartAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Stops a node of the specified address")]
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        var address = Address;
        var node = nodes.GetNodeOrCurrent(address);
        await node.StopAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Gets or sets the current node")]
    public void Current()
    {
        var address = Address;
        if (address != string.Empty && nodes.GetNodeOrCurrent(address) is { } node)
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
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Sends a transaction using a string")]
    public async Task TxAsync(
        [CommandSummary("Specifies the text to send")]
        string text,
        CancellationToken cancellationToken)
    {
        var address = Address;
        var node = nodes.GetNodeOrCurrent(address);
        var action = new StringAction { Value = text };
        await node.SendTransactionAsync([action], cancellationToken);
        await Out.WriteLineAsync($"{node.Address.ToShortString()}: {text}");
    }

    [CommandMethod("command")]
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Gets the command line to start a node")]
    public void GetCommandLine()
    {
        var address = Address;
        var node = nodes.GetNodeOrCurrent(address);
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

    private void ListNormal()
    {
        var tsb = new TerminalStringBuilder();
        for (var i = 0; i < nodes.Count; i++)
        {
            var item = nodes[i];
            var isCurrent = nodes.Current == item;
            tsb.Foreground = GetForeground(node: item, isCurrent);
            tsb.IsBold = item.IsRunning == true;
            tsb.AppendLine($"{item}");
            tsb.ResetOptions();
            tsb.Append(string.Empty);
        }

        Out.Write(tsb.ToString());
    }

    private void ListDetailed()
    {
        var infos = nodes.Select(node => node.Info).ToArray();
        Out.WriteLineAsJson(infos);
    }

    private string[] GetNodeAddresses()
        => nodes.Select(node => node.Address.ToString()).ToArray();
}
