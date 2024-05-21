using System.ComponentModel;
using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Nodes;

namespace LibplanetConsole.Banks.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Display balance information.")]
[Category("Bank")]
[method: ImportingConstructor]
internal sealed class BalanceCommand(INode node, IBankNode bankContent) : CommandAsyncBase
{
    public override bool IsEnabled => node.IsRunning is true;

    protected override async Task OnExecuteAsync(
        CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
    {
        var address = node.Address;
        var balanceInfo = await bankContent.GetBalanceAsync(address, cancellationToken);
        await Out.WriteLineAsJsonAsync(balanceInfo);
    }
}
