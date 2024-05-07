using System.ComponentModel.Composition;
using JSSoft.Commands;
using JSSoft.Terminals;
using LibplanetConsole.Clients;
using LibplanetConsole.Consoles;

namespace LibplanetConsole.ConsoleHost.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Provides client-related commands.")]
[method: ImportingConstructor]
internal sealed partial class ClientCommand(IApplication application, IClientCollection clients)
    : CommandMethodBase
{
    [CommandProperty(InitValue = 10)]
    public int Tick { get; set; }

    [CommandMethod]
    public void List()
    {
        var tsb = new TerminalStringBuilder();
        for (var i = 0; i < clients.Count; i++)
        {
            var item = clients[i];
            var isCurrent = clients.Current == item;
            tsb.Foreground = GetForeground(client: item, isCurrent);
            tsb.IsBold = item.IsRunning == true;
            tsb.AppendLine($"{item}");
            tsb.ResetOptions();
        }

        Out.Write(tsb.ToString());
    }

    [CommandMethod]
    public async Task StartAsync(string address, CancellationToken cancellationToken)
    {
        var client = application.GetClient(address);
        var node = GetRandomNode(application);
        var clientOptions = new ClientOptions()
        {
            NodeEndPoint = node.EndPoint,
        };
        await client.StartAsync(clientOptions, cancellationToken);
    }

    [CommandMethod]
    public async Task StopAsync(string address, CancellationToken cancellationToken)
    {
        var client = application.GetClient(address);
        await client.StopAsync(cancellationToken);
    }

    [CommandMethod]
    public void Current(string? address = null)
    {
        if (address is not null && application.GetClient(address) is { } client)
        {
            clients.Current = client;
        }

        if (clients.Current is not null)
        {
            Out.WriteLine(clients.Current);
        }
        else
        {
            Out.WriteLine("No client is selected.");
        }
    }

    private static TerminalColorType? GetForeground(IClient client, bool isCurrent)
    {
        if (client.IsRunning == true)
        {
            return isCurrent == true ? TerminalColorType.BrightGreen : null;
        }

        return TerminalColorType.BrightBlack;
    }

    private static INode GetRandomNode(IApplication application)
    {
        if (application.GetService(typeof(INodeCollection)) is not INodeCollection nodes)
        {
            throw new InvalidOperationException("The node collection is not found.");
        }

        if (nodes.Count == 0)
        {
            throw new InvalidOperationException("There is no node.");
        }

        var nodeIndex = Random.Shared.Next(nodes.Count);
        return nodes[nodeIndex];
    }
}
