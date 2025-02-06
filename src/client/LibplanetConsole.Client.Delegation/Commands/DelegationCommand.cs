using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Client.Delegation.Commands;

[CommandSummary("Delegation Commands.")]
[Category("Delegation")]
internal sealed class DelegationCommand(
    IClient client, IDelegation delegation)
    : CommandMethodBase
{
    [CommandMethod]
    public async Task StakeAsync(
        long ncg,
        CancellationToken cancellationToken)
    {
        await delegation.StakeAsync(ncg, cancellationToken);
    }

    [CommandMethod]
    public async Task DelegateeInfoAsync(
        Address address = default, CancellationToken cancellationToken = default)
    {
        var actualAddress = address == default ? client.Address : address;
        var info = await delegation.GetDelegateeInfoAsync(actualAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }

    [CommandMethod]
    public async Task DelegatorInfoAsync(
        Address address = default, CancellationToken cancellationToken = default)
    {
        var actualAddress = address == default ? client.Address : address;
        var info = await delegation.GetDelegatorInfoAsync(actualAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }

    [CommandMethod]
    public async Task StakeInfoAsync(
        Address address = default, CancellationToken cancellationToken = default)
    {
        var actualAddress = address == default ? client.Address : address;
        var info = await delegation.GetStakeInfoAsync(actualAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }
}
