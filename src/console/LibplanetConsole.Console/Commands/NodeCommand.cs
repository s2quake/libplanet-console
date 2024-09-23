using System.ComponentModel.Composition;
using JSSoft.Commands;
using JSSoft.Terminals;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Actions;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Console.Commands;

[Export(typeof(ICommand))]
[Export]
[CommandSummary("Provides node-related commands.")]
[method: ImportingConstructor]
public sealed partial class NodeCommand(ApplicationBase application, INodeCollection nodes)
    : CommandMethodBase
{
    [CommandPropertyRequired(DefaultValue = "")]
    [CommandSummary("The address of the client. If not specified, the current client is used.")]
    public static string Address { get; set; } = string.Empty;

    [CommandPropertySwitch("detail")]
    [CommandSummary("Displays the detailed information.")]
    public static bool IsDetailed { get; set; }

    [CommandMethod]
    [CommandMethodProperty(nameof(IsDetailed))]
    [CommandSummary("Displays the list of nodes.")]
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
    [CommandSummary("Deletes a node of the specified address.\n" +
                    "If the address is not specified, the current node is used.")]
    public async Task DeleteAsync()
    {
        var address = Address;
        var node = application.GetNode(address);
        await node.DisposeAsync();
    }

    [CommandMethod]
    [CommandSummary("Displays the node information of the specified address.\n" +
                    "If the address is not specified, the current node is used.")]
    public void Info()
    {
        var address = Address;
        var node = application.GetNode(address);
        var nodeInfo = InfoUtility.GetInfo(serviceProvider: application, obj: node);
        Out.WriteLineAsJson(nodeInfo);
    }

    [CommandMethod]
    [CommandSummary("Creates a new node.")]
    [CommandMethodStaticProperty(typeof(NewProperties))]
    public async Task NewAsync(
        string privateKey = "", CancellationToken cancellationToken = default)
    {
        var nodeOptions = new NodeOptions
        {
            EndPoint = AppEndPoint.Next(),
            PrivateKey = AppPrivateKey.ParseOrRandom(privateKey),
        };
        var options = new AddNewNodeOptions
        {
            NodeOptions = nodeOptions,
            NoProcess = NewProperties.NoProcess,
            Detach = NewProperties.Detach,
            NewWindow = NewProperties.NewWindow,
        };
        var node = await nodes.AddNewAsync(options, cancellationToken);
        var nodeInfo = node.Info;
        await Out.WriteLineAsJsonAsync(nodeInfo);
    }

    [CommandMethod]
    [CommandSummary("Attach to the node which is already running.\n" +
                    "If the address is not specified, the current node is used.")]
    public async Task AttachAsync(CancellationToken cancellationToken = default)
    {
        var address = Address;
        var node = application.GetNode(address);
        await node.AttachAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Detach from the node\n" +
                    "If the address is not specified, the current node is used.")]
    public async Task DetachAsync(CancellationToken cancellationToken = default)
    {
        var address = Address;
        var node = application.GetNode(address);
        await node.DetachAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Starts a node of the specified address.\n" +
                    "If the address is not specified, the current node is used.")]
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        var address = Address;
        var node = application.GetNode(address);
        await node.StartAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Stops a node of the specified address.\n" +
                    "If the address is not specified, the current node is used.")]
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        var address = Address;
        var node = application.GetNode(address);
        await node.StopAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Selects a node of the specified address.\n" +
                    "If the address is not specified, displays the current node.")]
    public void Current(string address = "")
    {
        if (address != string.Empty && application.GetNode(address) is { } node)
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
    [CommandSummary("Sends a transaction to store a simple string.\n" +
                    "If the address is not specified, the current node is used.")]
    public async Task TxAsync(
        [CommandSummary("The text to send.")]
        string text,
        CancellationToken cancellationToken)
    {
        var address = Address;
        var node = application.GetNode(address);
        var action = new StringAction { Value = text };
        await node.SendTransactionAsync([action], cancellationToken);
        await Out.WriteLineAsync($"{node.Address:S}: {text}");
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

    private static class NewProperties
    {
        [CommandPropertySwitch]
        [CommandSummary("The node is created but process is not executed.")]
        public static bool NoProcess { get; set; }

        [CommandPropertySwitch]
        [CommandSummary("The node is started in a new window.\n" +
                        "This option cannot be used with --no-process option.")]
        [CommandPropertyExclusion(nameof(NoProcess))]
        public static bool Detach { get; set; }

        [CommandPropertySwitch]
        [CommandSummary("The node is started in a new window.\n" +
                        "This option cannot be used with --no-process option.")]
        [CommandPropertyExclusion(nameof(NoProcess))]
        public static bool NewWindow { get; set; }
    }
}
