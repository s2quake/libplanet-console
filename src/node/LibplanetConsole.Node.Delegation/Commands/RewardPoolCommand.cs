using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Node.Delegation.Commands;

[CommandSummary("Display reward pool information.")]
[Category("Delegation")]
internal sealed class RewardPoolCommand(INode node, IDelegation delegation)
    : CommandAsyncBase()
{
    public override bool IsEnabled => node.IsRunning is true;

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var nodeAddress = node.Address;
        var amount = await delegation.GetRewardPoolAsync(nodeAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(new { NodeAddress = nodeAddress, Amount = $"{amount}" });
    }
}
