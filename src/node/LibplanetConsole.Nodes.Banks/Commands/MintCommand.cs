using System.ComponentModel;
using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Nodes;

namespace LibplanetConsole.Nodes.Banks.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Mint asset.")]
[Category("Bank")]
[method: ImportingConstructor]
internal sealed class MintCommand(INode node, IBankNode bankContent) : CommandAsyncBase
{
    public override bool IsEnabled => node.IsRunning is true;

    [CommandPropertyRequired(DefaultValue = 10)]
    public double Amount { get; set; }

    protected override async Task OnExecuteAsync(
        CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
    {
        var amount = Amount;
        var balanceInfo = await bankContent.MintAsync(amount, cancellationToken);
        await Out.WriteLineAsJsonAsync(balanceInfo);
    }
}
