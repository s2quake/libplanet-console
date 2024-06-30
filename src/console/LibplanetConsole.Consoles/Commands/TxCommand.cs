using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common.Actions;

namespace LibplanetConsole.Consoles.Commands;

[Export(typeof(ICommand))]
[method: ImportingConstructor]
[CommandSummary("Sends a transaction to store simple string.")]
internal sealed class TxCommand(ApplicationBase application) : CommandAsyncBase
{
    [CommandPropertyRequired]
    public string Address { get; set; } = string.Empty;

    [CommandPropertyRequired]
    public string Text { get; set; } = string.Empty;

    protected override async Task OnExecuteAsync(
        CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
    {
        var addressable = application.GetAddressable(Address);
        var text = Text;
        if (addressable is INode node)
        {
            var action = new StringAction { Value = text };
            await node.SendTransactionAsync([action], cancellationToken);
            await Out.WriteLineAsync($"{node.Address:S}: {text}");
        }
        else if (addressable is IClient client)
        {
            await client.SendTransactionAsync(text, cancellationToken);
            await Out.WriteLineAsync($"{client.Address:S}: {text}");
        }
        else
        {
            throw new InvalidOperationException("Invalid addressable.");
        }
    }
}
