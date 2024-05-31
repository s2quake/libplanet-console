using System.ComponentModel;
using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Nodes.Banks.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Burn NCG.")]
[Category("Bank")]
[method: ImportingConstructor]
internal sealed class BurnCommand(INode node, IBankNode bankContent) : CommandAsyncBase
{
    public override bool IsEnabled => node.IsRunning is true;

    [CommandPropertyRequired(DefaultValue = 10)]
    public double Amount { get; set; }

    protected override async Task OnExecuteAsync(
        CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
    {
        var amount = Amount;
        var balanceInfo = await bankContent.BurnAsync(amount, cancellationToken);
        await Out.WriteLineAsJsonAsync(balanceInfo);
    }
}
