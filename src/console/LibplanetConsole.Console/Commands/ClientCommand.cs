using JSSoft.Commands;
using JSSoft.Terminals;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Actions;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Console.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Commands;

[CommandSummary("Provides client-related commands.")]
public sealed partial class ClientCommand(
    IServiceProvider serviceProvider, IClientCollection clients)
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
        var client = clients.GetClientOrCurrent(address);
        var clientInfo = InfoUtility.GetInfo(serviceProvider, obj: client);
        Out.WriteLineAsJson(clientInfo);
    }

    [CommandMethod]
    [CommandSummary("Attach to the client which is already running.\n" +
                    "If the address is not specified, current client is used.")]
    [CommandMethodProperty(nameof(Address))]
    public async Task AttachAsync(CancellationToken cancellationToken = default)
    {
        var address = Address;
        var client = clients.GetClientOrCurrent(address);
        await client.AttachAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Detach from the client.\n" +
                    "If the address is not specified, current client is used.")]
    [CommandMethodProperty(nameof(Address))]
    public async Task DetachAsync(CancellationToken cancellationToken = default)
    {
        var address = Address;
        var client = clients.GetClientOrCurrent(address);
        await client.DetachAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Starts a client of the specified address.\n" +
                    "If the address is not specified, current client is used.")]
    [CommandMethodProperty(nameof(Address))]
    public async Task StartAsync(
        string nodeAddress = "", CancellationToken cancellationToken = default)
    {
        var nodes = serviceProvider.GetRequiredService<NodeCollection>();
        var address = Address;
        var node = nodes.GetNodeOrCurrent(nodeAddress);
        var client = clients.GetClientOrCurrent(address);
        await client.StartAsync(node, cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Stops a client of the specified address.\n" +
                    "If the address is not specified, current client is used.")]
    [CommandMethodProperty(nameof(Address))]
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        var address = Address;
        var client = clients.GetClientOrCurrent(address);
        await client.StopAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Selects a client of the specified address.\n" +
                    "If the address is not specified, displays the current client.")]
    [CommandMethodProperty(nameof(Address))]
    public void Current()
    {
        var address = Address;
        if (address != string.Empty && clients.GetClientOrCurrent(address) is { } client)
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
        var client = clients.GetClientOrCurrent(address);
        var blockChain = client.GetRequiredService<IBlockChain>();
        var action = new StringAction { Value = text };
        await blockChain.SendTransactionAsync([action], cancellationToken);
        await Out.WriteLineAsync($"{client.Address.ToShortString()}: {text}");
    }

    [CommandMethod("command")]
    public void GetCommandLine()
    {
        var address = Address;
        var client = clients.GetClientOrCurrent(address);
        Out.WriteLine(client.GetCommandLine());
    }

    private static TerminalColorType? GetForeground(IClient client, bool isCurrent)
    {
        if (client.IsRunning == true)
        {
            return isCurrent == true ? TerminalColorType.BrightGreen : null;
        }

        return TerminalColorType.BrightBlack;
    }
}
