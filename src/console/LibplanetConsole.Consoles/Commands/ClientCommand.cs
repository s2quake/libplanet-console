using System.ComponentModel.Composition;
using JSSoft.Commands;
using JSSoft.Terminals;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Consoles.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Provides client-related commands.")]
[method: ImportingConstructor]
internal sealed partial class ClientCommand(IApplication application, IClientCollection clients)
    : CommandMethodBase
{
    [CommandPropertyRequired(DefaultValue = "")]
    [CommandSummary("The address of the client. If not specified, the current client is used.")]
    public static string Address { get; set; } = string.Empty;

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
    [CommandMethodStaticProperty(typeof(StartProperties))]
    public async Task NewAsync(
        string privateKey = "", CancellationToken cancellationToken = default)
    {
        var options = new AddNewOptions
        {
            PrivateKey = PrivateKeyUtility.ParseWithFallback(privateKey),
            ManualStart = StartProperties.ManualStart,
            NewWindow = StartProperties.NewWindow,
            Detached = StartProperties.Detached,
        };
        var client = await clients.AddNewAsync(options, cancellationToken);
        var clientInfo = client.Info;
        await Out.WriteLineAsJsonAsync(clientInfo);
    }

    [CommandMethod]
    [CommandSummary("Deletes a client of the specified address.\n" +
                    "If the address is not specified, current client is used.")]
    [CommandMethodProperty(nameof(Address))]
    public async Task DeleteAsync()
    {
        var address = Address;
        var client = application.GetClient(address);
        await client.DisposeAsync();
    }

    [CommandMethod]
    [CommandSummary("Starts a client of the specified address.\n" +
                    "If the address is not specified, current client is used.")]
    [CommandMethodProperty(nameof(Address))]
    public async Task StartAsync(
        string nodeAddress = "", CancellationToken cancellationToken = default)
    {
        var nodes = application.GetService<NodeCollection>();
        var address = Address;
        var node = nodeAddress == string.Empty
            ? nodes.RandomNode()
            : application.GetNode(nodeAddress);
        var client = application.GetClient(address);
        await client.StartAsync(node, cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Stops a client of the specified address.\n" +
                    "If the address is not specified, current client is used.")]
    [CommandMethodProperty(nameof(Address))]
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        var address = Address;
        var client = application.GetClient(address);
        await client.StopAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Selects a client of the specified address.\n" +
                    "If the address is not specified, displays the current client.")]
    [CommandMethodProperty(nameof(Address))]
    public void Current()
    {
        var address = Address;
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

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Sends a transaction to store a simple string.\n" +
                    "If the address is not specified, current client is used.")]
    public async Task TxAsync(
        [CommandSummary("The text to send.")]
        string text,
        CancellationToken cancellationToken)
    {
        var address = Address;
        var client = application.GetClient(address);
        await client.SendTransactionAsync(text, cancellationToken);
        Out.WriteLine($"{(ShortAddress)client.Address}: {text}");
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

    private static class StartProperties
    {
        [CommandPropertySwitch("new-window", 'n')]
        public static bool NewWindow { get; set; }

        [CommandPropertySwitch("manual-start", 'm')]
        public static bool ManualStart { get; set; }

        [CommandPropertySwitch("detach", 'd')]
        public static bool Detached { get; set; }
    }
}
