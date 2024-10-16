using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Client.Bank.Commands;

[CommandSummary("Display balance information.")]
[Category("Bank")]
internal sealed class BalanceCommand(IClient client, IBank bank) : CommandAsyncBase
{
    public override bool IsEnabled => client.IsRunning is true;

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var address = client.Address;
        var balanceInfo = await bank.GetBalanceAsync(address, cancellationToken);
        await Out.WriteLineAsJsonAsync(balanceInfo);
    }
}
