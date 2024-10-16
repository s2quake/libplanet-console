using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Node.Bank.Commands;

[CommandSummary("Transfer NCG to specific address.")]
[Category("Bank")]
internal sealed class TransferCommand(INode node, IBank bank) : CommandAsyncBase
{
    public override bool IsEnabled => node.IsRunning is true;

    [CommandPropertyRequired]
    public string TargetAddress { get; set; } = string.Empty;

    [CommandPropertyRequired(DefaultValue = 10)]
    public decimal Amount { get; set; }

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var targetAddress = new Address(TargetAddress);
        var amount = Amount;
        var balanceInfo = await bank.TransferAsync(
            targetAddress: targetAddress,
            amount: amount,
            cancellationToken: cancellationToken);
        await Out.WriteLineAsJsonAsync(balanceInfo);
    }
}
