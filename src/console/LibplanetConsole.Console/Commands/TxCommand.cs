using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Actions;
using LibplanetConsole.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Sends a transaction using a simple string.")]
internal sealed class TxCommand(ApplicationBase application) : CommandAsyncBase
{
    [CommandPropertyRequired]
    public string Address { get; set; } = string.Empty;

    [CommandPropertyRequired]
    public string Text { get; set; } = string.Empty;

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var addressable = application.GetAddressable(Address);
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
            await client.SendTransactionAsync(text, cancellationToken);
            await Out.WriteLineAsync($"{client.Address.ToShortString()}: {text}");
        }
        else
        {
            throw new InvalidOperationException("Invalid addressable.");
        }
    }
}
