using JSSoft.Commands;
using LibplanetConsole.Common.Actions;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Node.Commands;

[CommandSummary("Sends a transaction using a string")]
internal sealed class TxCommand(INode node, IBlockChain blockChain) : CommandAsyncBase
{
    [CommandPropertyRequired]
    [CommandSummary("Specifies the text to send")]
    public string Text { get; set; } = string.Empty;

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var action = new StringAction
        {
            Value = Text,
        };
        await blockChain.SendTransactionAsync([action], cancellationToken);
        await Out.WriteLineAsync($"{node.Address.ToShortString()}: {Text}");
    }
}
