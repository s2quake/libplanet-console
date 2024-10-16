using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Node.Bank.Commands;

[CommandSummary("Retrieve pool information.")]
[Category("Bank")]
internal sealed partial class PoolCommand(INode node, IBank bank) : CommandAsyncBase
{
    public override bool IsEnabled => node.IsRunning is true;

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var poolInfo = await bank.GetPoolAsync(cancellationToken);
        await Out.WriteLineAsJsonAsync(poolInfo);
    }
}
