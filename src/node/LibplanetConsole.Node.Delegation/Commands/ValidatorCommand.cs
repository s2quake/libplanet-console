using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Node.Delegation.Commands;

[CommandSummary("Display validator information.")]
[Category("Delegation")]
internal sealed class ValidatorCommand(INode node, IDelegation delegation)
    : CommandAsyncBase()
{
    public override bool IsEnabled => node.IsRunning is true;

    [CommandPropertyRequired(DefaultValue = new string[] { })]
    public string[] Validator { get; set; } = [];

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var validatorInfos = await delegation.GetValidatorsAsync(cancellationToken);
        await Out.WriteLineAsJsonAsync(validatorInfos);
    }
}
