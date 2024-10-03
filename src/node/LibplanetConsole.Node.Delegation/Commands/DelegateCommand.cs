using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Node.Delegation.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Delegate NCG to validator for delegation.")]
[Category("Delegation")]
internal sealed class DelegateCommand(INode node, IDelegation delegation)
    : CommandAsyncBase()
{
    public override bool IsEnabled => node.IsRunning is true;

    [CommandPropertyRequired(DefaultValue = 10)]
    public double Amount { get; set; }

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var nodeAddress = node.Address;
        var amount = Amount;
        var validatorInfo = await delegation.DelegateAsync(
            nodeAddress: nodeAddress,
            amount: amount,
            cancellationToken: cancellationToken);
        await Out.WriteLineAsJsonAsync(validatorInfo);
    }
}
