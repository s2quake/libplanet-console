using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Console.Bank;

namespace LibplanetConsole.Console.Delegation.Commands;

[CommandSummary("Delegation Commands.")]
[Category("Delegation")]
internal sealed class DelegationCommand(IDelegation delegation, IBank bank) : CommandMethodBase
{
    [CommandMethod]
    public async Task StakeAsync(
        long ncg,
        CancellationToken cancellationToken)
    {
        await delegation.StakeAsync(ncg, cancellationToken);
    }
}
