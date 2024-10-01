using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Node.Bank.Commands;

[CommandSummary("Burn NCG.")]
[Category("Bank")]
internal sealed class BurnCommand(INode node, IBank bank) : CommandAsyncBase
{
    public override bool IsEnabled => node.IsRunning is true;

    [CommandPropertyRequired(DefaultValue = 10)]
    public decimal Amount { get; set; }

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var amount = Amount;
        var balanceInfo = await bank.BurnAsync(amount, cancellationToken);
        await Out.WriteLineAsJsonAsync(balanceInfo);
    }
}
