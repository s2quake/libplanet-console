using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Client.Bank.Commands;

[CommandSummary("Mint asset.")]
[Category("Bank")]
internal sealed class MintCommand(IClient client, IBank bank) : CommandAsyncBase
{
    public override bool IsEnabled => client.IsRunning is true;

    [CommandPropertyRequired(DefaultValue = 10)]
    public decimal Amount { get; set; }

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var amount = Amount;
        var balanceInfo = await bank.MintAsync(amount, cancellationToken);
        await Out.WriteLineAsJsonAsync(balanceInfo);
    }
}
