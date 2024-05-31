using System.ComponentModel;
using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Nodes.Banks.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Transfer NCG to specific address.")]
[Category("Bank")]
[method: ImportingConstructor]
internal sealed class TransferCommand(INode node, IBankNode bankContent) : CommandAsyncBase
{
    public override bool IsEnabled => node.IsRunning is true;

    [CommandPropertyRequired]
    public string TargetAddress { get; set; } = string.Empty;

    [CommandPropertyRequired(DefaultValue = 10)]
    public double Amount { get; set; }

    protected override async Task OnExecuteAsync(
        CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
    {
        var targetAddress = AddressUtility.Parse(TargetAddress);
        var amount = Amount;
        var balanceInfo = await bankContent.TransferAsync(
            targetAddress: targetAddress,
            amount: amount,
            cancellationToken: cancellationToken);
        await Out.WriteLineAsJsonAsync(balanceInfo);
    }
}
