using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Client.Delegation.Commands;

[CommandSummary("Display validator information.")]
[Category("Delegation")]
internal sealed class ValidatorCommand(IClient client, IDelegation delegation)
    : CommandAsyncBase()
{
    public override bool IsEnabled => client.IsRunning is true;

    [CommandPropertyRequired(DefaultValue = new string[] { })]
    public string[] Validator { get; set; } = [];

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var validatorInfos = await delegation.GetValidatorsAsync(cancellationToken);
        await Out.WriteLineAsJsonAsync(validatorInfos);
    }
}
