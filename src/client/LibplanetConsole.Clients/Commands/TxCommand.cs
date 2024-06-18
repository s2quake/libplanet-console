using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Actions;

namespace LibplanetConsole.Clients.Commands;

[Export(typeof(ICommand))]
[method: ImportingConstructor]
[CommandSummary("Sends a transaction to store simple string.")]
internal sealed class TxCommand(IClient client) : CommandAsyncBase
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
        await client.SendTransactionAsync([action], cancellationToken);
        await Out.WriteLineAsync($"{(ShortAddress)client.Address}: {Text}");
    }
}
