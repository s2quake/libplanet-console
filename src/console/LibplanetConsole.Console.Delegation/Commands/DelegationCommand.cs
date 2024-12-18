using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Console.Delegation.Commands;

[CommandSummary("Delegation Commands.")]
[Category("Delegation")]
internal sealed class DelegationCommand(
    IConsole console, IDelegation delegation) : CommandMethodBase
{
    [CommandMethod]
    public async Task StakeAsync(long ncg, CancellationToken cancellationToken)
    {
        await delegation.StakeAsync(ncg, cancellationToken);
    }

    [CommandMethod]
    public async Task DelegateeInfoAsync(
        string address = "", CancellationToken cancellationToken = default)
    {
        var targetAddress = address == string.Empty ? console.Address : new Address(address);
        var info = await delegation.GetDelegateeInfoAsync(targetAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }

    [CommandMethod]
    public async Task DelegatorInfoAsync(
        string address = "", CancellationToken cancellationToken = default)
    {
        var targetAddress = address == string.Empty ? console.Address : new Address(address);
        var info = await delegation.GetDelegatorInfoAsync(targetAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }

    public async Task StakeInfoAsync(
        string address = "", CancellationToken cancellationToken = default)
    {
        var targetAddress = address == string.Empty ? console.Address : new Address(address);
        var info = await delegation.GetStakeInfoAsync(targetAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }
}
