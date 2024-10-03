using JSSoft.Commands;
using LibplanetConsole.Common.Actions;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Node.Commands;

[CommandSummary("Adds a transaction to store simple string.")]
internal sealed class TxCommand(INode node, IBlockChain blockChain) : CommandAsyncBase
{
    [CommandPropertyRequired]
    public string Text { get; set; } = string.Empty;

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var action = new StringAction
        {
            Value = Text,
        };
        await blockChain.AddTransactionAsync([action], cancellationToken);
        await Out.WriteLineAsync($"{node.Address.ToShortString()}: {Text}");
    }
}
