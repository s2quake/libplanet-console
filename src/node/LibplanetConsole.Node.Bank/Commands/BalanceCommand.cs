using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Node.Bank.Commands;

[CommandSummary("Display balance information.")]
[Category("Bank")]
internal sealed class BalanceCommand(INode node, IBank bank, BankCommand bankCommand)
    : CommandAsyncBase(bankCommand)
{
    public override bool IsEnabled => node.IsRunning is true;

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var address = node.Address;
        var balanceInfo = await bank.GetBalanceAsync(address, cancellationToken);
        await Out.WriteLineAsJsonAsync(balanceInfo);
    }
}
