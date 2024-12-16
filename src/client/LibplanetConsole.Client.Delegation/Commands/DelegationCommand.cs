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
    public async Task InfoAsync(string address = "", CancellationToken cancellationToken = default)
    {
        var delegatorAddress = address == string.Empty ? client.Address : new Address(address);
        var info = await delegation.GetDelegatorInfoAsync(delegatorAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }
}
