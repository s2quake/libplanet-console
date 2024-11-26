using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Node.Delegation.Commands;

[CommandSummary("Delegation Commands.")]
[Category("Delegation")]
internal sealed class DelegationCommand(Delegation delegation) : CommandMethodBase
{
    [CommandMethod]
    public async Task StakeAsync(long amount, CancellationToken cancellationToken)
    {
        await delegation.StakeAsync(amount, cancellationToken);
    }

    [CommandMethod]
    public async Task PromoteAsync(long amount, CancellationToken cancellationToken)
    {
        await delegation.PromoteAsync(amount, cancellationToken);
    }
}
