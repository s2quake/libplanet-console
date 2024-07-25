using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common.Actions;

namespace LibplanetConsole.Clients.Commands;

[Export(typeof(ICommand))]
[method: ImportingConstructor]
[CommandSummary("Sends a transaction to store simple string.")]
internal sealed class TxCommand(IClient client, IBlockChain blockChain) : CommandAsyncBase
{
    [CommandPropertyRequired]
    public string Text { get; set; } = string.Empty;

    protected override async Task OnExecuteAsync(
        CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
    {
        var action = new StringAction
        {
            Value = Text,
        };
        await blockChain.SendTransactionAsync([action], cancellationToken);
        await Out.WriteLineAsync($"{client.Address:S}: {Text}");
    }
}
