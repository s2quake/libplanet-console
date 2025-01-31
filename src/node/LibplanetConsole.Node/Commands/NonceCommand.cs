using JSSoft.Commands;
using LibplanetConsole.BlockChain;

namespace LibplanetConsole.Node.Commands;

[CommandSummary("Gets a nonce from the node.")]
internal sealed class NonceCommand(INode node, IBlockChain blockChain) : CommandAsyncBase
{
    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var address = node.Address;
        var nonce = await blockChain.GetNextNonceAsync(address, cancellationToken);
        await Out.WriteLineAsync($"{nonce}");
    }
}
