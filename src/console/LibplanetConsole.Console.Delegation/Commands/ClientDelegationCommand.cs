using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Console.Commands;

namespace LibplanetConsole.Console.Delegation.Commands;

internal sealed partial class ClientDelegationCommand(
    IServiceProvider serviceProvider,
    ClientCommand clientCommand)
    : ClientCommandMethodBase(serviceProvider, clientCommand, "delegation")
{
    [CommandMethod]
    [CommandMethodProperty(nameof(ClientAddress))]
    public async Task StakeAsync(
        long ncg,
        CancellationToken cancellationToken)
    {
        var client = CurrentClient;
        var delegation = client.GetRequiredKeyedService<IClientDelegation>(IClient.Key);
        await delegation.StakeAsync(ncg, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(ClientAddress))]
    public async Task DelegateeInfoAsync(
        [CommandParameterCompletion(nameof(GetNodeAddresses))]
        Address address = default,
        CancellationToken cancellationToken = default)
    {
        var client = CurrentClient;
        var actualAddress = address == default ? client.Address : address;
        var delegation = client.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        var info = await delegation.GetDelegateeInfoAsync(actualAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(ClientAddress))]
    public async Task DelegatorInfoAsync(
        [CommandParameterCompletion(nameof(GetDelegatorAddresses))]
        Address address = default,
        CancellationToken cancellationToken = default)
    {
        var client = CurrentClient;
        var actualAddress = address == default ? client.Address : address;
        var delegation = client.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        var info = await delegation.GetDelegatorInfoAsync(actualAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(ClientAddress))]
    public async Task StakeInfoAsync(
        [CommandParameterCompletion(nameof(GetAllAddresses))]
        Address address = default,
        CancellationToken cancellationToken = default)
    {
        var client = CurrentClient;
        var actualAddress = address == default ? client.Address : address;
        var delegation = client.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        var info = await delegation.GetStakeInfoAsync(actualAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }

    private string[] GetDelegatorAddresses() => GetAddresses(IConsole.Tag, IClient.Tag);

    private string[] GetNodeAddresses() => GetAddresses(INode.Tag);

    private string[] GetAllAddresses() => GetAddresses(IConsole.Tag, INode.Tag, IClient.Tag);
}
