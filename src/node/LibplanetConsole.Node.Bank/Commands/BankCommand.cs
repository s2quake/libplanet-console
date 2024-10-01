using System.ComponentModel;
using JSSoft.Commands;

namespace LibplanetConsole.Node.Bank.Commands;

[CommandSummary("Bank Commands.")]
[Category("Bank")]
internal sealed class BankCommand(IBank bank) : CommandMethodBase
{
    [CommandMethod("init-supply")]
    public async Task GetInitialSupplyAsync(CancellationToken cancellationToken)
    {
        var amount = await bank.GetInitialSupplyAsync(cancellationToken);
        await Out.WriteLineAsync($"{amount}");
    }

    [CommandMethod("supply")]
    public async Task GetSupplyAsync(CancellationToken cancellationToken)
    {
        var amount = await bank.GetSupplyAsync(cancellationToken);
        await Out.WriteLineAsync($"{amount}");
    }
}
