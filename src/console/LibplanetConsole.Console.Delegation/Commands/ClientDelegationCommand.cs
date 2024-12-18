using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Console.Commands;
using LibplanetConsole.Console.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Delegation.Commands;

internal sealed partial class ClientDelegationCommand(
    ClientCommand clientCommand,
    IClientCollection clients)
    : CommandMethodBase(clientCommand, "delegation")
{
    [CommandProperty(InitValue = "")]
    [CommandSummary("The address of the client. If not specified, the current client is used.")]
    [CommandPropertyCompletion(nameof(GetClientAddresses))]
    public static string ClientAddress { get; set; } = string.Empty;

    [CommandMethod]
    [CommandMethodProperty(nameof(ClientAddress))]
    public async Task StakeAsync(
        long ncg,
        CancellationToken cancellationToken)
    {
        var address = ClientAddress;
        var client = clients.GetClientOrCurrent(address);
        var delegation = client.GetRequiredKeyedService<IClientDelegation>(IClient.Key);
        await delegation.StakeAsync(ncg, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(ClientAddress))]
    public async Task DelegateeInfoAsync(
        string address = "", CancellationToken cancellationToken = default)
    {
        var client = clients.GetClientOrCurrent(ClientAddress);
        var targetAddress = address == string.Empty ? client.Address : new Address(address);
        var delegation = client.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        var info = await delegation.GetDelegateeInfoAsync(targetAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(ClientAddress))]
    public async Task DelegatorInfoAsync(
       string address = "", CancellationToken cancellationToken = default)
    {
        var client = clients.GetClientOrCurrent(ClientAddress);
        var targetAddress = address == string.Empty ? client.Address : new Address(address);
        var delegation = client.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        var info = await delegation.GetDelegatorInfoAsync(targetAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(ClientAddress))]
    public async Task StakeInfoAsync(
        string address = "", CancellationToken cancellationToken = default)
    {
        var client = clients.GetClientOrCurrent(ClientAddress);
        var targetAddress = address == string.Empty ? client.Address : new Address(address);
        var delegation = client.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        var info = await delegation.GetStakeInfoAsync(targetAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }

    private string[] GetClientAddresses()
        => [.. clients.Select(client => client.Address.ToString())];
}
