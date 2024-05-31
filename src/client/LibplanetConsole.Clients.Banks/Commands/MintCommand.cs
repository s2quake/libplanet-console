using System.ComponentModel;
using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Clients.Banks.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Mint asset.")]
[Category("Bank")]
[method: ImportingConstructor]
internal sealed class MintCommand(IClient client, IBankClient bankContent) : CommandAsyncBase
{
    public override bool IsEnabled => client.IsRunning is true;

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
