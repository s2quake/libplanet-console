using System.ComponentModel;
using JSSoft.Commands;

namespace LibplanetConsole.Node.Delegation.Commands;

[CommandSummary("Claim NCG from validator.")]
[Category("Delegation")]
internal sealed class ClaimCommand(INode node, IDelegation delegation)
    : CommandAsyncBase()
{
    public override bool IsEnabled => node.IsRunning is true;

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var nodeAddress = node.Address;
        await delegation.ClaimAsync(
            nodeAddress: nodeAddress,
            cancellationToken: cancellationToken);
    }
}
