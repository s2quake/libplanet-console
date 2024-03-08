using System.ComponentModel.Composition;
using JSSoft.Commands;
using JSSoft.Terminals;
using LibplanetConsole.Executable.Extensions;
using LibplanetConsole.Executable.Serializations;

namespace LibplanetConsole.Executable.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Provides commands related to swarms.")]
[method: ImportingConstructor]
sealed class SwarmCommand(Application application, SwarmHostCollection swarmHosts) : CommandMethodBase
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
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.SwarmIndex))]
    public void Info()
    {
        var swarmHost = application.GetSwarmHost(IndexProperties.SwarmIndex);
        var swarmInfo = new SwarmInfo(swarmHost.Target);
        Out.WriteLineAsJson(swarmInfo);
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.SwarmIndex))]
    public async Task Start(CancellationToken cancellationToken)
    {
        var seedPeer = swarmHosts[0].Peer;
        var consensusSeedPeer = swarmHosts[0].ConsensusPeer;
        var swarmHost = application.GetSwarmHost(IndexProperties.SwarmIndex);
        await swarmHost.StartAsync(seedPeer, consensusSeedPeer, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.SwarmIndex))]
    public async Task Stop(CancellationToken cancellationToken)
    {
        var swarmHost = application.GetSwarmHost(IndexProperties.SwarmIndex);
        await swarmHost.StopAsync(cancellationToken);
    }

    [CommandMethod]
    public void Current(int? value = null)
    {
        if (value is { } index)
        {
            swarmHosts.Current = swarmHosts[index];
        }
        else
        {
            Out.WriteLine(swarmHosts.IndexOf(swarmHosts.Current));
        }
    }

    private void ListNormal()
    {
        var tsb = new TerminalStringBuilder();
        for (var i = 0; i < swarmHosts.Count; i++)
        {
            var item = swarmHosts[i];
            var isCurrent = swarmHosts.Current == item;
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
        for (var i = 0; i < swarmHosts.Count; i++)
        {
            var item = swarmHosts[i];
            var swarmInfo = new SwarmInfo(item.Target);
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
