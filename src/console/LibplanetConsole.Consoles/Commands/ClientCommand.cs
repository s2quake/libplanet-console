using System.ComponentModel.Composition;
using JSSoft.Commands;
using JSSoft.Terminals;
using LibplanetConsole.Clients;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Consoles.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Provides client-related commands.")]
[method: ImportingConstructor]
internal sealed partial class ClientCommand(IApplication application, IClientCollection clients)
    : CommandMethodBase
{
    [CommandMethod]
    [CommandSummary("Displays the list of clients.")]
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
            tsb.Append(string.Empty);
        }

        Out.Write(tsb.ToString());
    }

    [CommandMethod]
    [CommandSummary("Creates a new client.")]
    public async Task NewAsync(CancellationToken cancellationToken)
    {
        var client = await clients.AddNewAsync(cancellationToken);
        var clientInfo = client.Info;
        await Out.WriteLineAsJsonAsync(clientInfo);
    }

    [CommandMethod]
    [CommandSummary("Deletes a client of the specified address.\n" +
                    "If the address is not specified, current client is used.")]
    public async Task DeleteAsync(string address = "")
    {
        var client = application.GetClient(address);
        await client.DisposeAsync();
    }

    [CommandMethod]
    [CommandSummary("Starts a client of the specified address.\n" +
                    "If the address is not specified, current client is used.")]
    public async Task StartAsync(string address = "", CancellationToken cancellationToken = default)
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
    [CommandSummary("Stops a client of the specified address.\n" +
                    "If the address is not specified, current client is used.")]
    public async Task StopAsync(string address = "", CancellationToken cancellationToken = default)
    {
        var client = application.GetClient(address);
        await client.StopAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Selects a client of the specified address.\n" +
                    "If the address is not specified, displays the current client.")]
    public void Current(string address = "")
    {
        if (address != string.Empty && application.GetClient(address) is { } client)
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
