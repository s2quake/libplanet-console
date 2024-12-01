using JSSoft.Commands;
using LibplanetConsole.Common.Actions;

namespace LibplanetConsole.Console.Commands;

[CommandSummary("Sends a transaction using a string")]
internal sealed class TxCommand(IConsole console) : CommandAsyncBase
{
    [CommandPropertyRequired]
    [CommandSummary("Specifies the text to send")]
    public string Text { get; set; } = string.Empty;

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var text = Text;
        var action = new StringAction { Value = text };
        await console.SendTransactionAsync([action], cancellationToken);
        await Out.WriteLineAsync($"{console.Address}: {text}");
    }
}
