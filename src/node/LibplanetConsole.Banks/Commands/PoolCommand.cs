using System.ComponentModel;
using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Nodes;

namespace LibplanetConsole.Banks.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Retrieve pool information.")]
[Category("Bank")]
[method: ImportingConstructor]
internal sealed partial class PoolCommand(INode node, IBankNode bankContent) : CommandAsyncBase
{
    public override bool IsEnabled => node.IsRunning is true;

    protected override async Task OnExecuteAsync(
        CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
    {
        var poolInfo = await bankContent.GetPoolAsync(cancellationToken);
        await Out.WriteLineAsJsonAsync(poolInfo);
    }
}
