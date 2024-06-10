using System.ComponentModel;
using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Nodes.Stakings.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Display validator information.")]
[Category("Staking")]
[method: ImportingConstructor]
internal sealed class ValidatorCommand(INode node, IStakingNode staking)
    : CommandAsyncBase()
{
    public override bool IsEnabled => node.IsRunning is true;

    [CommandPropertyRequired(DefaultValue = new string[] { })]
    public string[] Validator { get; set; } = [];

    protected override async Task OnExecuteAsync(
        CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
    {
        var validatorInfos = await staking.GetValidatorsAsync(cancellationToken);
        await Out.WriteLineAsJsonAsync(validatorInfos);
    }
}
