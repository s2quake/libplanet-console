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
    [CommandPropertyRequired(DefaultValue = "")]
    [CommandSummary("The address of the client. If not specified, the current client is used.")]
    [CommandPropertyCompletion(nameof(GetClientAddresses))]
    public static string Address { get; set; } = string.Empty;

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    public async Task StakeAsync(
        long ncg,
        CancellationToken cancellationToken)
    {
        var address = Address;
        var client = clients.GetClientOrCurrent(address);
        var delegation = client.GetRequiredKeyedService<IClientDelegation>(IClient.Key);
        await delegation.StakeAsync(ncg, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    public async Task InfoAsync(CancellationToken cancellationToken)
    {
        var address = Address;
        var client = clients.GetClientOrCurrent(address);
        var delegation = client.GetRequiredKeyedService<IClientDelegation>(IClient.Key);
        var info = await delegation.GetDelegatorInfoAsync(client.Address, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }

    private string[] GetClientAddresses()
        => [.. clients.Select(client => client.Address.ToString())];
}
