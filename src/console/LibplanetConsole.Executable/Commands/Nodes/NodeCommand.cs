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
internal sealed partial class NodeCommand(IApplication application, INodeCollection nodes)
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
        var nodeInfo = node.Info;
        await Out.WriteLineAsJsonAsync(nodeInfo);
    }

    [CommandMethod]
    public async Task InfoAsync(string address, CancellationToken cancellationToken)
    {
        var node = application.GetNode(address);
        var nodeInfo = await node.GetInfoAsync(cancellationToken);
        await Out.WriteLineAsJsonAsync(nodeInfo);
    }

    [CommandMethod]
    public async Task Start(string address, CancellationToken cancellationToken)
    {
        // var node = application.GetNode(address);
        // await node.StartAsync(cancellationToken);
        await Task.CompletedTask;
    }

    [CommandMethod]
    public async Task Stop(string address, CancellationToken cancellationToken)
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
    public void Current(string? address = null)
    {
        if (address is not null && application.GetNode(address) is { } node)
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
