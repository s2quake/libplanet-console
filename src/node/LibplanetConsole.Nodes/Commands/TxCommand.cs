using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common.Actions;

namespace LibplanetConsole.Nodes.Commands;

[Export(typeof(ICommand))]
[method: ImportingConstructor]
[CommandSummary("Adds a transaction to store simple string.")]
internal sealed class TxCommand(INode node, IBlockChain blockChain) : CommandAsyncBase
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
        await blockChain.AddTransactionAsync([action], cancellationToken);
        await Out.WriteLineAsync($"{node.Address:S}: {Text}");
    }
}
