using JSSoft.Commands;
using JSSoft.Terminals;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Actions;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Console.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Commands;

[CommandSummary("Provides client-related commands")]
public sealed partial class ClientCommand(
    IServiceProvider serviceProvider, IClientCollection clients)
    : CommandMethodBase
{
    [CommandPropertyRequired(DefaultValue = "")]
    [CommandPropertyCompletion(nameof(GetClientAddresses))]
    [CommandSummary("Specifies the address of the client.")]
    public static string Address { get; set; } = string.Empty;

    [CommandMethod]
    [CommandSummary("Displays the list of clients")]
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
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Displays a client information")]
    public void Info()
    {
        var address = Address;
        var client = clients.GetClientOrCurrent(address);
        var clientInfo = InfoUtility.GetInfo(serviceProvider, obj: client);
        Out.WriteLineAsJson(clientInfo);
    }

    [CommandMethod]
    [CommandSummary("Attaches the client instance to the client process")]
    [CommandMethodProperty(nameof(Address))]
    public async Task AttachAsync(CancellationToken cancellationToken = default)
    {
        var address = Address;
        var client = clients.GetClientOrCurrent(address);
        await client.AttachAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Detaches the client instance from the client process")]
    public async Task DetachAsync(CancellationToken cancellationToken = default)
    {
        var address = Address;
        var client = clients.GetClientOrCurrent(address);
        await client.DetachAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Starts the client")]
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
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Stops the client")]
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        var address = Address;
        var client = clients.GetClientOrCurrent(address);
        await client.StopAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Gets or sets the current client")]
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
            Out.WriteLine("No client is selected");
        }
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Sends a transaction using a string")]
    public async Task TxAsync(
        [CommandSummary("Specifies the text to send")]
        string text,
        CancellationToken cancellationToken)
    {
        var address = Address;
        var client = clients.GetClientOrCurrent(address);
        var action = new StringAction { Value = text };
        await client.SendTransactionAsync([action], cancellationToken);
        await Out.WriteLineAsync($"{client.Address.ToShortString()}: {text}");
    }

    [CommandMethod("command")]
    [CommandSummary("Gets the command line to start a client")]
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

    private string[] GetClientAddresses()
        => clients.Select(client => client.Address.ToString()).ToArray();
}
