using System.ComponentModel.Composition;
using JSSoft.Library.Commands;
using JSSoft.Library.Terminals;
using OnBoarding.ConsoleHost.Serializations;

namespace OnBoarding.ConsoleHost.Commands;

[Export(typeof(ICommand))]
[method: ImportingConstructor]
sealed class SwarmCommand(Application application, SwarmHostCollection swarmHosts) : CommandMethodBase
{
    private readonly Application _application = application;
    private readonly SwarmHostCollection _swarmHosts = swarmHosts;

    [CommandProperty('i', useName: true, InitValue = -1)]
    public int Index { get; set; }

    [CommandMethod]
    public void List()
    {
        var tsb = new TerminalStringBuilder();
        for (var i = 0; i < _swarmHosts.Count; i++)
        {
            var item = _swarmHosts[i];
            tsb.Foreground = item.IsRunning == true ? null : TerminalColorType.BrightBlack;
            tsb.IsBold = item.IsRunning == true;
            tsb.AppendLine($"[{i}]-{item}");
            tsb.Foreground = null;
            tsb.IsBold = false;
            tsb.Append(string.Empty);
        }
        Out.Write(tsb.ToString());
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(Index))]
    public void Info()
    {
        var swarmHost = Index == -1 ? _swarmHosts[_application.CurrentIndex] : _swarmHosts[Index];
        var swarmInfo = new SwarmInfo(swarmHost.Target);
        var json = JsonUtility.SerializeObject(swarmInfo, isColorized: true);
        Out.Write(json);
    }

    // [CommandMethod]
    // [CommandMethodProperty(nameof(Count))]
    // public async Task NewAsync(CancellationToken cancellationToken)
    // {
    //     var taskList = new List<Task>(Count);
    //     var itemList = new List<SwarmHost>(Count);
    //     var users = _application.GetService<UserCollection>()!;
    //     var sb = new StringBuilder();
    //     for (var i = 0; i < Count; i++)
    //     {
    //         var privateKey = new PrivateKey();
    //         var blockChain = BlockChainUtils.CreateBlockChain([.. users]);
    //         var swarmHost = _swarmHosts.AddNew(privateKey, blockChain);
    //         var task = swarmHost.StartAsync(cancellationToken);
    //         taskList.Add(task);
    //         itemList.Add(swarmHost);
    //     }
    //     await Task.WhenAll(taskList);
    //     foreach (var item in itemList)
    //     {
    //         var index = _swarmHosts.IndexOf(item);
    //         sb.AppendLine($"[{index}]-{item} has been created.");
    //     }
    //     Out.Write(sb.ToString());
    // }

    // [CommandMethod]
    // [CommandMethodProperty(nameof(Index))]
    // public async Task DeleteAsync(CancellationToken cancellationToken)
    // {
    //     var swarmHost = Index == -1 ? _swarmHosts[_application.CurrentIndex] : _swarmHosts[Index];
    //     var swarmIndex = _swarmHosts.IndexOf(swarmHost);
    //     await swarmHost.DisposeAsync();
    //     Out.WriteLine($"[{swarmIndex}]-{swarmHost} has been deleted.");
    // }

    // [CommandMethod]
    // [CommandMethodProperty(nameof(Index))]
    // public async Task StartAsync(CancellationToken cancellationToken)
    // {
    //     var swarmHost = Index == -1 ? _swarmHosts[_application.CurrentIndex] : _swarmHosts[Index];
    //     var swarmIndex = _swarmHosts.IndexOf(swarmHost);
    //     await swarmHost.StartAsync(cancellationToken);
    //     Out.WriteLine($"[{swarmIndex}]-{swarmHost} has been started.");
    // }

    // [CommandMethod]
    // [CommandMethodProperty(nameof(Index))]
    // public async Task StopAsync(CancellationToken cancellationToken)
    // {
    //     var swarmHost = Index == -1 ? _swarmHosts[_application.CurrentIndex] : _swarmHosts[Index];
    //     var swarmIndex = _swarmHosts.IndexOf(swarmHost);
    //     await swarmHost.StopAsync(cancellationToken);
    //     Out.WriteLine($"[{swarmIndex}]-{swarmHost} has been stopped.");
    // }
}
