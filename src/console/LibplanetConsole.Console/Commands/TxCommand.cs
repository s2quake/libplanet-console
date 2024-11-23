using JSSoft.Commands;
using LibplanetConsole.Common.Actions;
using LibplanetConsole.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Commands;

[CommandSummary("Sends a transaction using a string")]
internal sealed class TxCommand(INodeCollection nodes, IClientCollection clients) : CommandAsyncBase
{
    [CommandPropertyRequired]
    [CommandSummary("Specifies the address of the node or client")]
    public string Address { get; set; } = string.Empty;

    [CommandPropertyRequired]
    [CommandSummary("Specifies the text to send")]
    public string Text { get; set; } = string.Empty;

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var addressable = GetAddressable(new(Address));
        var text = Text;
        if (addressable is INode node)
        {
            var blockChain = node.GetRequiredService<IBlockChain>();
            var action = new StringAction { Value = text };
            await blockChain.SendTransactionAsync([action], cancellationToken);
            await Out.WriteLineAsync($"{node.Address.ToShortString()}: {text}");
        }
        else if (addressable is IClient client)
        {
            var blockChain = client.GetRequiredService<IBlockChain>();
            var action = new StringAction { Value = text };
            await blockChain.SendTransactionAsync([action], cancellationToken);
            await Out.WriteLineAsync($"{client.Address.ToShortString()}: {text}");
        }
        else
        {
            throw new InvalidOperationException("Invalid addressable.");
        }
    }

    private IAddressable GetAddressable(Address address)
    {
        if (nodes.Contains(address) is true)
        {
            return nodes[address];
        }

        if (clients.Contains(address) is true)
        {
            return clients[address];
        }

        throw new ArgumentException("Invalid address.");
    }
}
