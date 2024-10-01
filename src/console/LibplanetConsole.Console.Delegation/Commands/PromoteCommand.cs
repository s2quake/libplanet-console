using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Delegation.Services;

namespace LibplanetConsole.Console.Delegation.Commands;

[CommandSummary("Promote validator with NCG.")]
[Category("Delegation")]
internal sealed class PromoteCommand(IApplication application)
    : CommandAsyncBase()
{
    [CommandPropertyRequired]
    public string NodeAddress { get; set; } = string.Empty;

    [CommandProperty("amount", 'a', InitValue = 10)]
    public double Amount { get; set; }

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var node = application.GetNode(NodeAddress);
        if (node.GetService(typeof(IDelegation)) is IDelegation delegation)
        {
            var options = new PromoteOptions
            {
                Amount = Amount,
            };
            var validatorInfo = await delegation.PromoteAsync(options, cancellationToken);
            await Out.WriteLineAsJsonAsync(validatorInfo);
        }
        else
        {
            throw new InvalidOperationException("Delegation service is not available.");
        }
    }
}
