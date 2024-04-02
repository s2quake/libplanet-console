using System.ComponentModel.Composition;
using JSSoft.Commands;
using JSSoft.Terminals;
using LibplanetConsole.Executable.Extensions;
using LibplanetConsole.Executable.Serializations;

namespace LibplanetConsole.Executable.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Provides node-related commands.")]
[method: ImportingConstructor]
sealed class NodeCommand(Application application, NodeCollection nodes) : CommandMethodBase
{
    [CommandPropertySwitch("detail")]
    public bool IsDetailed { get; set; }

    [CommandMethod]
    [CommandMethodProperty(nameof(IsDetailed))]
    public void List()
    {
        GetListAction(IsDetailed).Invoke();

        Action GetListAction(bool IsDetailed) => IsDetailed switch
        {
            false => ListNormal,
            true => ListDetailed,
        };
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.NodeIndex))]
    public void Info()
    {
        var node = application.GetNode(IndexProperties.NodeIndex);
        var swarmInfo = new NodeInfo(node.Target);
        Out.WriteLineAsJson(swarmInfo);
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.NodeIndex))]
    public async Task Start(CancellationToken cancellationToken)
    {
        var seedPeer = nodes[0].Peer;
        var consensusSeedPeer = nodes[0].ConsensusPeer;
        var node = application.GetNode(IndexProperties.NodeIndex);
        await node.StartAsync(seedPeer, consensusSeedPeer, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.NodeIndex))]
    public async Task Stop(CancellationToken cancellationToken)
    {
        var node = application.GetNode(IndexProperties.NodeIndex);
        await node.StopAsync(cancellationToken);
    }

    [CommandMethod]
    public void Current(int? value = null)
    {
        if (value is { } index)
        {
            nodes.Current = nodes[index];
        }
        else
        {
            Out.WriteLine(nodes.IndexOf(nodes.Current));
        }
    }

    private void ListNormal()
    {
        var tsb = new TerminalStringBuilder();
        for (var i = 0; i < nodes.Count; i++)
        {
            var item = nodes[i];
            var isCurrent = nodes.Current == item;
            var blockChain = item.Target.BlockChain;
            tsb.Foreground = item.IsRunning == true ? (isCurrent == true ? TerminalColorType.BrightGreen : null) : TerminalColorType.BrightBlack;
            tsb.IsBold = item.IsRunning == true;
            tsb.AppendLine($"[{i}] {item}");
            tsb.ResetOptions();
            tsb.AppendLine($"  BlockCount: {blockChain.Count}");
            tsb.Append(string.Empty);
        }
        Out.Write(tsb.ToString());
    }

    private void ListDetailed()
    {
        var tsb = new TerminalStringBuilder();
        for (var i = 0; i < nodes.Count; i++)
        {
            var item = nodes[i];
            var swarmInfo = new NodeInfo(item.Target);
            var json = JsonUtility.SerializeObject(swarmInfo, isColorized: true);
            tsb.Foreground = item.IsRunning == true ? null : TerminalColorType.BrightBlack;
            tsb.IsBold = item.IsRunning == true;
            tsb.AppendLine($"[{i}] {item}");
            tsb.ResetOptions();
            tsb.Append(json);
        }
        Out.Write(tsb.ToString());
    }
}
