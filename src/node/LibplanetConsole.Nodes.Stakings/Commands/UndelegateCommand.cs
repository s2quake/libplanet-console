using System.ComponentModel;
using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Nodes;

namespace LibplanetConsole.Nodes.Stakings.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Undelegate Share from validator.")]
[Category("Staking")]
[method: ImportingConstructor]
internal sealed class UndelegateCommand(INode node, IStakingNode stakingContent)
    : CommandAsyncBase()
{
    public override bool IsEnabled => node.IsRunning is true;

    [CommandPropertyRequired(DefaultValue = 500)]
    public long ShareAmount { get; set; }

    protected override async Task OnExecuteAsync(
        CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
    {
        var nodeAddress = node.Address;
        var shareAmount = ShareAmount;
        var validatorInfo = await stakingContent.UndelegateAsync(
            nodeAddress: nodeAddress,
            shareAmount: shareAmount,
            cancellationToken: cancellationToken);
        await Out.WriteLineAsJsonAsync(validatorInfo);
    }
}
