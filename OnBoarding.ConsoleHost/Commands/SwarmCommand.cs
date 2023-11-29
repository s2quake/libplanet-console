using System.Collections;
using System.ComponentModel.Composition;
using System.Text;
using JSSoft.Library.Commands;
using Libplanet.Net;
using Newtonsoft.Json.Linq;

namespace OnBoarding.ConsoleHost.Commands;

[Export(typeof(ICommand))]
[method: ImportingConstructor]
sealed class SwarmCommand(SwarmHostCollection swarmHosts) : CommandMethodBase
{
    private readonly SwarmHostCollection _swarmHosts = swarmHosts;

    [CommandProperty(InitValue = 1)]
    public int Count { get; set; }

    [CommandMethod]
    public void List()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < _swarmHosts.Count; i++)
        {
            var item = _swarmHosts[i];
            var isOpen = item.IsRunning == true ? "O" : " ";
            sb.AppendLine($"{isOpen} [{i}]-{item.Key}");
        }
        Out.Write(sb.ToString());
    }

    [CommandMethod]
    public void Info(string key)
    {
        var swarmHost = int.TryParse(key, out var index) == true ? _swarmHosts[index] : _swarmHosts[key];
        var swarm = swarmHost.Target;
        var json = new JObject
        {
            { "AppProtocolVersion", $"{swarm.AppProtocolVersion}" },
            { "Address", $"{swarm.Address}" },
            { "ConsensusRunning", $"{swarm.ConsensusRunning}" },
            { "Running", $"{swarm.Running}" },
            { "LastMessageTimestamp", $"{swarm.LastMessageTimestamp}" },
            {
                "BlockChain", new JObject
                {
                    { "Id", $"{swarm.BlockChain.Id}" },
                    { "Tip", $"{swarm.BlockChain.Tip}" },
                    { "Count", $"{swarm.BlockChain.Count}" },
                }
            },
        };
        Out.Write(json.ToString());
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(Count))]
    public async Task NewAsync(CancellationToken cancellationToken)
    {
        var taskList = new List<Task>(Count);
        var itemList = new List<SwarmHost>(Count);
        var sb = new StringBuilder();
        for (var i = 0; i < Count; i++)
        {
            var swarmHost = _swarmHosts.AddNew();
            var task = swarmHost.StartAsync(cancellationToken);
            taskList.Add(task);
            itemList.Add(swarmHost);
        }
        await Task.WhenAll(taskList);
        foreach (var item in itemList)
        {
            var index = _swarmHosts.IndexOf(item);
            sb.AppendLine($"[{index}]-{item.Key} has been created.");
        }
        Out.Write(sb.ToString());
    }

    [CommandMethod]
    public async Task DeleteAsync(string key, CancellationToken cancellationToken)
    {
        var swarmHost = int.TryParse(key, out var index) == true ? _swarmHosts[index] : _swarmHosts[key];
        var swarmIndex = _swarmHosts.IndexOf(swarmHost);
        await swarmHost.DisposeAsync();
        Out.WriteLine($"[{swarmIndex}]-{swarmHost.Key} has been deleted.");
    }

    [CommandMethod]
    public async Task StartAsync(string key, CancellationToken cancellationToken)
    {
        var swarmHost = int.TryParse(key, out var index) == true ? _swarmHosts[index] : _swarmHosts[key];
        var swarmIndex = _swarmHosts.IndexOf(swarmHost);
        await swarmHost.StartAsync(cancellationToken);
        Out.WriteLine($"[{swarmIndex}]-{swarmHost.Key} has been started.");
    }

    [CommandMethod]
    public async Task StopAsync(string key, CancellationToken cancellationToken)
    {
        var swarmHost = int.TryParse(key, out var index) == true ? _swarmHosts[index] : _swarmHosts[key];
        var swarmIndex = _swarmHosts.IndexOf(swarmHost);
        await swarmHost.StopAsync(cancellationToken);
        Out.WriteLine($"[{swarmIndex}]-{swarmHost.Key} has been stopped.");
    }
}
