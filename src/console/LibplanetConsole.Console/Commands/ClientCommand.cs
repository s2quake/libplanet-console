using JSSoft.Commands;
using JSSoft.Terminals;
using LibplanetConsole.BlockChain;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Actions;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Console.Extensions;

namespace LibplanetConsole.Console.Commands;

[CommandSummary("Provides client-related commands")]
public sealed partial class ClientCommand(
    IServiceProvider serviceProvider, IClientCollection clients, IAddressCollection addresses)
    : ClientCommandMethodBase(serviceProvider), IExecutable
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    [CommandMethod]
    [CommandSummary("Displays the list of clients")]
    public void List()
    {
        var tsb = new TerminalStringBuilder();
        for (var i = 0; i < clients.Count; i++)
        {
            var client = clients[i];
            var address = client.Address;
            var isCurrent = clients.Current == client;
            tsb.Foreground = GetForeground(client: client, isCurrent);
            tsb.IsBold = client.IsRunning == true;
            if (addresses.TryGetAlias(address, out var alias) is true)
            {
                tsb.AppendLine($"{client} ({alias})");
            }
            else
            {
                tsb.AppendLine($"{client}");
            }

            tsb.ResetOptions();
            tsb.Append(string.Empty);
        }

        Out.Write(tsb.ToString());
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(ClientAddress))]
    [CommandSummary("Displays the client information of the specified address")]
    public void Info()
    {
        var client = GetClientOrCurrent(ClientAddress);
        var clientInfo = InfoUtility.GetInfo(_serviceProvider, obj: client);
        Out.WriteLineAsJson(clientInfo);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(ClientAddress))]
    [CommandSummary("Attaches the client instance to the client process")]
    public async Task AttachAsync(CancellationToken cancellationToken = default)
    {
        var client = GetClientOrCurrent(ClientAddress);
        await client.AttachAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(ClientAddress))]
    [CommandSummary("Detaches the client instance from the client process")]
    public async Task DetachAsync(CancellationToken cancellationToken = default)
    {
        var client = GetClientOrCurrent(ClientAddress);
        await client.DetachAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(ClientAddress))]
    [CommandSummary("Starts the client")]
    public async Task StartAsync(
        [CommandParameterCompletion(nameof(GetNodeAddresses))]
        Address nodeAddress = default,
        CancellationToken cancellationToken = default)
    {
        var nodes = _serviceProvider.GetRequiredService<NodeCollection>();
        var node = nodes.GetNodeOrCurrent(nodeAddress);
        var client = GetClientOrCurrent(ClientAddress);
        await client.StartAsync(node, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(ClientAddress))]
    [CommandSummary("Stops the client")]
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        var client = GetClientOrCurrent(ClientAddress);
        await client.StopAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Gets or sets the current client")]
    public void Current(
        [CommandSummary("The address of the client")]
        [CommandParameterCompletion(nameof(GetClientAddresses))]
        Address clientAddress = default)
    {
        var client = GetClientOrDefault(clientAddress);
        if (client is not null)
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
    [CommandMethodProperty(nameof(ClientAddress))]
    [CommandSummary("Sends a transaction using a string")]
    public async Task TxAsync(
        [CommandSummary("Specifies the text to send")]
        string text,
        CancellationToken cancellationToken)
    {
        var client = GetClientOrCurrent(ClientAddress);
        var action = new StringAction { Value = text };
        await client.SendTransactionAsync([action], cancellationToken);
        await Out.WriteLineAsync($"{client.Address.ToShortString()}: {text}");
    }

    [CommandMethod("command")]
    [CommandMethodProperty(nameof(ClientAddress))]
    [CommandSummary("Gets the command line to start a client")]
    public void GetCommandLine()
    {
        var client = GetClientOrCurrent(ClientAddress);
        Out.WriteLine(client.GetCommandLine());
    }

    void IExecutable.Execute()
    {
        if (Context.HelpCommand is HelpCommandBase helpCommand)
        {
            helpCommand.PrintHelp(this);
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

    private string[] GetNodeAddresses() => GetAddresses(INode.Tag);
}
