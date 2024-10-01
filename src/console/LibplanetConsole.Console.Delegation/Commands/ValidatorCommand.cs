using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Console.Delegation.Commands;

[CommandSummary("Display validator information.")]
[Category("Delegation")]
internal sealed partial class ValidatorCommand(INodeCollection nodes)
    : CommandAsyncBase()
{
    [CommandPropertyRequired(DefaultValue = new string[] { })]
    public string[] Validator { get; set; } = [];

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var node = nodes.Current ?? throw new InvalidOperationException("Node is not set.");
        if (node.GetService(typeof(IDelegation)) is IDelegation delegation)
        {
            var validatorInfos = await delegation.GetValidatorsAsync(cancellationToken);
            await Out.WriteLineAsJsonAsync(validatorInfos);
        }
        else
        {
            throw new InvalidOperationException("Delegation service is not available.");
        }
    }
}
