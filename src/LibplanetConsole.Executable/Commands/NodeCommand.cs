using System.ComponentModel.Composition;
using JSSoft.Commands;
using JSSoft.Communication;
using JSSoft.Terminals;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Executable.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Provides node-related commands.")]
[method: ImportingConstructor]
internal sealed class NodeCommand(IApplication application, NodeCollection nodes)
    : CommandMethodBase
{
    [CommandPropertySwitch("detail")]
    public bool IsDetailed { get; set; }

    [CommandProperty("promote", 'p', DefaultValue = 10)]
    public double PromoteAmount { get; set; }

    [CommandMethod]
    [CommandMethodProperty(nameof(IsDetailed))]
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
    [CommandMethodProperty(nameof(PromoteAmount))]
    public async Task NewAsync(CancellationToken cancellationToken)
    {
        var node = await nodes.AddNewAsync(cancellationToken);
        var nodeInfo = await node.GetInfoAsync(cancellationToken);
        await Out.WriteLineAsJsonAsync(nodeInfo);
    }

    [CommandMethod]
    public async Task InfoAsync(string identifier, CancellationToken cancellationToken)
    {
        var node = application.GetNode(identifier);
        var nodeInfo = await node.GetInfoAsync(cancellationToken);
        await Out.WriteLineAsJsonAsync(nodeInfo);
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.Node))]
    public async Task Start(CancellationToken cancellationToken)
    {
        // var seedPeer = nodes[0].Peer;
        // var consensusSeedPeer = nodes[0].ConsensusPeer;
        // var node = application.GetNode(IndexProperties.NodeIndex);
        // await node.StartAsync(cancellationToken);
        await Task.CompletedTask;
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.Node))]
    public async Task Stop(CancellationToken cancellationToken)
    {
        var node = application.GetNode(IndexProperties.Node);
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
    public void Current(string? identifier = null)
    {
        if (identifier is not null && application.GetNode(identifier) is { } node)
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
        // var tsb = new TerminalStringBuilder();
        // for (var i = 0; i < nodes.Count; i++)
        // {
        //     var item = nodes[i];
        //     var swarmInfo = new NodeInfo(item.Target);
        //     var json = JsonUtility.SerializeObject(swarmInfo, isColorized: true);
        //     tsb.Foreground = item.IsRunning == true ? null : TerminalColorType.BrightBlack;
        //     tsb.IsBold = item.IsRunning == true;
        //     tsb.AppendLine($"[{i}] {item}");
        //     tsb.ResetOptions();
        //     tsb.Append(json);
        // }
        // Out.Write(tsb.ToString());
    }
}
