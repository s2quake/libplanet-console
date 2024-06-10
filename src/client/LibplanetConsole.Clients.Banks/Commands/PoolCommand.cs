using System.ComponentModel;
using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Clients.Banks.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Retrieve pool information.")]
[Category("Bank")]
[method: ImportingConstructor]
internal sealed partial class PoolCommand(
    IClient client, IBankClient bankContent) : CommandAsyncBase
{
    public override bool IsEnabled => client.IsRunning is true;

    protected override async Task OnExecuteAsync(
        CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
    {
        var poolInfo = await bankContent.GetPoolAsync(cancellationToken);
        await Out.WriteLineAsJsonAsync(poolInfo);
    }
}
