using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Node.Delegation.Commands;

[CommandSummary("Undelegate Share from validator.")]
[Category("Delegation")]
internal sealed class UndelegateCommand(INode node, IDelegation delegation)
    : CommandAsyncBase()
{
    public override bool IsEnabled => node.IsRunning is true;

    [CommandPropertyRequired(DefaultValue = 500)]
    public long ShareAmount { get; set; }

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var nodeAddress = node.Address;
        var shareAmount = ShareAmount;
        var validatorInfo = await delegation.UndelegateAsync(
            nodeAddress: nodeAddress,
            shareAmount: shareAmount,
            cancellationToken: cancellationToken);
        await Out.WriteLineAsJsonAsync(validatorInfo);
    }
}
