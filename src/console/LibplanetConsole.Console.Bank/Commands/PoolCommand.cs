using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Console.Bank.Commands;

[CommandSummary("Retrieve pool information.")]
[Category("Bank")]
internal sealed partial class PoolCommand(INodeCollection nodes) : CommandAsyncBase
{
    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var node = nodes.Current ?? throw new InvalidOperationException("Node is not set.");
        if (node.GetService(typeof(IBank)) is IBank bank)
        {
            var poolInfo = await bank.GetPoolAsync(cancellationToken);
            await Out.WriteLineAsJsonAsync(poolInfo);
        }
        else
        {
            throw new InvalidOperationException("Bank service is not available.");
        }
    }
}
