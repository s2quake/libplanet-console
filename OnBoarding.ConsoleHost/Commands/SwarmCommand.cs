using System.ComponentModel.Composition;
using JSSoft.Library.Commands;
using JSSoft.Library.Terminals;
using OnBoarding.ConsoleHost.Extensions;
using OnBoarding.ConsoleHost.Serializations;

namespace OnBoarding.ConsoleHost.Commands;

[Export(typeof(ICommand))]
sealed class SwarmCommand : CommandMethodBase
{
    private readonly Application _application;
    private readonly SwarmHostCollection _swarmHosts;

    [ImportingConstructor]
    public SwarmCommand(Application application, SwarmHostCollection swarmHosts)
    {
        _application = application;
        _swarmHosts = swarmHosts;
    }

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
        var swarmHost = _application.GetSwarmHost(IndexProperties.SwarmIndex);
        var swarmInfo = new SwarmInfo(swarmHost.Target);
        Out.WriteLineAsJson(swarmInfo);
    }

    private void ListNormal()
    {
        var tsb = new TerminalStringBuilder();
        for (var i = 0; i < _swarmHosts.Count; i++)
        {
            var item = _swarmHosts[i];
            var blockChain = item.Target.BlockChain;
            tsb.Foreground = item.IsRunning == true ? null : TerminalColorType.BrightBlack;
            tsb.IsBold = item.IsRunning == true;
            tsb.AppendLine($"[{i}] {item}");
            tsb.Foreground = null;
            tsb.IsBold = false;
            tsb.AppendLine($"  BlockCount: {blockChain.Count}");
            tsb.Append(string.Empty);
        }
        Out.Write(tsb.ToString());
    }

    private void ListDetailed()
    {
        var tsb = new TerminalStringBuilder();
        for (var i = 0; i < _swarmHosts.Count; i++)
        {
            var item = _swarmHosts[i];
            var swarmInfo = new SwarmInfo(item.Target);
            tsb.Foreground = item.IsRunning == true ? null : TerminalColorType.BrightBlack;
            tsb.IsBold = item.IsRunning == true;
            tsb.AppendLine($"[{i}] {item}");
            tsb.Foreground = null;
            tsb.IsBold = false;
            var json = JsonUtility.SerializeObject(swarmInfo, isColorized: true);
            tsb.Append(json);
        }
        Out.Write(tsb.ToString());
    }
}
