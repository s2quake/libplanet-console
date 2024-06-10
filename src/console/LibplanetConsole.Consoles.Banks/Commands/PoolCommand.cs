using System.ComponentModel;
using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Consoles.Banks.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Retrieve pool information.")]
[Category("Bank")]
[method: ImportingConstructor]
internal sealed partial class PoolCommand(INodeCollection nodes) : CommandAsyncBase
{
    protected override async Task OnExecuteAsync(
        CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
    {
        var node = nodes.Current ?? throw new InvalidOperationException("Node is not set.");
        if (node.GetService(typeof(IBankContent)) is IBankContent bankContent)
        {
            var poolInfo = await bankContent.GetPoolAsync(cancellationToken);
            await Out.WriteLineAsJsonAsync(poolInfo);
        }
        else
        {
            throw new InvalidOperationException("Bank service is not available.");
        }
    }
}
