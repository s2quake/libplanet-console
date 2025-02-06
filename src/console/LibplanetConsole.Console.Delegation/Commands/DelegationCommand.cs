using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Alias;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Console.Delegation.Commands;

[CommandSummary("Delegation Commands.")]
[Category("Delegation")]
internal sealed class DelegationCommand(
    IConsole console,
    IDelegation delegation,
    IAliasCollection aliases) : CommandMethodBase
{
    [CommandMethod]
    public async Task StakeAsync(long ncg, CancellationToken cancellationToken)
    {
        await delegation.StakeAsync(ncg, cancellationToken);
    }

    [CommandMethod]
    public async Task DelegateeInfoAsync(
        [CommandParameterCompletion(nameof(GetNodeAddresses))]
        Address address = default,
        CancellationToken cancellationToken = default)
    {
        var actualAddress = address == default ? console.Address : address;
        var info = await delegation.GetDelegateeInfoAsync(actualAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }

    [CommandMethod]
    public async Task DelegatorInfoAsync(
        [CommandParameterCompletion(nameof(GetDelegatorAddresses))]
        Address address = default,
        CancellationToken cancellationToken = default)
    {
        var actualAddress = address == default ? console.Address : address;
        var info = await delegation.GetDelegatorInfoAsync(actualAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }

    [CommandMethod]
    public async Task StakeInfoAsync(
        [CommandParameterCompletion(nameof(GetAllAddresses))]
        Address address = default,
        CancellationToken cancellationToken = default)
    {
        var actualAddress = address == default ? console.Address : address;
        var info = await delegation.GetStakeInfoAsync(actualAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }

    private string[] GetDelegatorAddresses() => aliases.GetAddresses(IConsole.Tag, IClient.Tag);

    private string[] GetNodeAddresses() => aliases.GetAddresses(INode.Tag);

    private string[] GetAllAddresses()
        => aliases.GetAddresses(IConsole.Tag, INode.Tag, IClient.Tag);
}
