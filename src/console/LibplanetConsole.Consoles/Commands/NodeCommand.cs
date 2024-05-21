using System.ComponentModel.Composition;
using JSSoft.Commands;
using JSSoft.Terminals;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Consoles.Services;
using LibplanetConsole.Nodes;

namespace LibplanetConsole.Consoles.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Provides node-related commands.")]
[method: ImportingConstructor]
internal sealed partial class NodeCommand(ApplicationBase application, INodeCollection nodes)
    : CommandMethodBase
{
    [CommandPropertySwitch("detail")]
    public bool IsDetailed { get; set; }

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
    [CommandSummary("Creates a new node.")]
    public async Task NewAsync(CancellationToken cancellationToken)
    {
        var node = await nodes.AddNewAsync(cancellationToken);
        var nodeInfo = node.Info;
        await Out.WriteLineAsJsonAsync(nodeInfo);
    }

    [CommandMethod]
    [CommandSummary("Deletes a node of the specified address.\n" +
                    "If the address is not specified, the current node is used.")]
    public async Task DeleteAsync(string address = "")
    {
        var node = application.GetNode(address);
        await node.DisposeAsync();
    }

    [CommandMethod]
    [CommandSummary("Displays the node information of the specified address.\n" +
                    "If the address is not specified, the current node is used.")]
    public void Info(string address = "")
    {
        var node = application.GetNode(address);
        var nodeInfo = node.Info;
        Out.WriteLineAsJson(nodeInfo);
    }

    [CommandMethod]
    [CommandSummary("Starts a node of the specified address.\n" +
                    "If the address is not specified, the current node is used.")]
    public async Task Start(string address = "", CancellationToken cancellationToken = default)
    {
        var node = application.GetNode(address);
        var seedService = application.GetService<SeedService>();
        var nodeOptions = new NodeOptions
        {
            GenesisOptions = application.GenesisOptions,
            BlocksyncSeedPeer = seedService.BlocksyncSeedPeer,
            ConsensusSeedPeer = seedService.ConsensusSeedPeer,
        };
        await node.StartAsync(nodeOptions, cancellationToken);
        await Task.CompletedTask;
    }

    [CommandMethod]
    [CommandSummary("Stops a node of the specified address.\n" +
                    "If the address is not specified, the current node is used.")]
    public async Task Stop(string address = "", CancellationToken cancellationToken = default)
    {
        var node = application.GetNode(address);
        await node.StopAsync(cancellationToken);
    }

    [CommandMethod]
    public async Task AttachAsync(
        string endPoint, string privateKey, CancellationToken cancellationToken)
    {
        await nodes.AttachAsync(
            endPoint: EndPointUtility.Parse(endPoint),
            privateKey: PrivateKeyUtility.Parse(privateKey),
            cancellationToken: cancellationToken);
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
}
