using System.ComponentModel.Composition;
using JSSoft.Commands;
using JSSoft.Terminals;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Consoles.Commands;

[Export(typeof(ICommand))]
[Export]
[CommandSummary("Provides client-related commands.")]
[method: ImportingConstructor]
public sealed partial class ClientCommand(ApplicationBase application, IClientCollection clients)
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
    [CommandSummary("Displays the client information of the specified address.\n" +
                    "If the address is not specified, the current client is used.")]
    public void Info()
    {
        var address = Address;
        var client = application.GetClient(address);
        var clientInfo = InfoUtility.GetInfo(serviceProvider: application, obj: client);
        Out.WriteLineAsJson(clientInfo);
    }

    [CommandMethod]
    [CommandSummary("Creates a new client.")]
    [CommandMethodStaticProperty(typeof(NewProperties))]
    public async Task NewAsync(
        string privateKey = "", CancellationToken cancellationToken = default)
    {
        var options = new AddNewOptions
        {
            PrivateKey = AppPrivateKey.ParseOrRandom(privateKey),
            NoProcess = NewProperties.NoProcess,
            Detach = NewProperties.Detach,
            NewWindow = NewProperties.NewWindow,
            ManualStart = NewProperties.ManualStart,
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
    [CommandSummary("Attach to the client which is already running.\n" +
                    "If the address is not specified, current client is used.")]
    [CommandMethodProperty(nameof(Address))]
    public async Task AttachAsync(CancellationToken cancellationToken = default)
    {
        var address = Address;
        var client = application.GetClient(address);
        await client.AttachAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Detach from the client.\n" +
                    "If the address is not specified, current client is used.")]
    [CommandMethodProperty(nameof(Address))]
    public async Task DetachAsync(CancellationToken cancellationToken = default)
    {
        var address = Address;
        var client = application.GetClient(address);
        await client.DetachAsync(cancellationToken);
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
    [CommandSummary("Display command line to execute the client process.")]
    public void CommandLine()
    {
        var address = Address;
        var client = application.GetClient(address);
        var clientProcess = client.CreateProcess();
        var nodes = application.GetService<NodeCollection>();
        var node = nodes.RandomNode();
        clientProcess.Detach = true;
        clientProcess.NewWindow = true;
        clientProcess.NodeEndPoint = node.EndPoint;
        Out.WriteLine(clientProcess.GetCommandLine());
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
        await Out.WriteLineAsync($"{client.Address:S}: {text}");
    }

    private static TerminalColorType? GetForeground(IClient client, bool isCurrent)
    {
        if (client.IsRunning == true)
        {
            return isCurrent == true ? TerminalColorType.BrightGreen : null;
        }

        return TerminalColorType.BrightBlack;
    }

    private static class NewProperties
    {
        [CommandPropertySwitch]
        [CommandSummary("The client is created but process is not executed.")]
        public static bool NoProcess { get; set; }

        [CommandPropertySwitch]
        [CommandSummary("The client is started in a new window.\n" +
                        "This option cannot be used with --no-process option.")]
        [CommandPropertyCondition(nameof(NoProcess), false)]
        public static bool Detach { get; set; }

        [CommandPropertySwitch]
        [CommandSummary("The client is started in a new window.\n" +
                        "This option cannot be used with --no-process option.")]
        [CommandPropertyCondition(nameof(NoProcess), false)]
        public static bool NewWindow { get; set; }

        [CommandPropertySwitch('m', useName: true)]
        [CommandSummary("The service does not start automatically " +
                        "when the client process is executed.\n" +
                        "This option cannot be used with --no-process or --detach option.")]
        [CommandPropertyCondition(nameof(NoProcess), false)]
        [CommandPropertyCondition(nameof(Detach), false)]
        public static bool ManualStart { get; set; }
    }
}
