using System.ComponentModel;
using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Clients;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Banks.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Display balance information.")]
[Category("Bank")]
[method: ImportingConstructor]
internal sealed class BalanceCommand(IClient client, IBankClient bankContent) : CommandAsyncBase
{
    public override bool IsEnabled => client.IsRunning is true;

    protected override async Task OnExecuteAsync(
        CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
    {
        var address = client.Address;
        var balanceInfo = await bankContent.GetBalanceAsync(address, cancellationToken);
        await Out.WriteLineAsJsonAsync(balanceInfo);
    }
}
