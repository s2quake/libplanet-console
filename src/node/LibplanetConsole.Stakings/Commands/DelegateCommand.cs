using System.ComponentModel;
using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Nodes;

namespace LibplanetConsole.Stakings.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Delegate NCG to validator for staking.")]
[Category("Staking")]
[method: ImportingConstructor]
internal sealed class DelegateCommand(INode node, IStakingNode stakingContent)
    : CommandAsyncBase()
{
    public override bool IsEnabled => node.IsRunning is true;

    [CommandPropertyRequired(DefaultValue = 10)]
    public double Amount { get; set; }

    protected override async Task OnExecuteAsync(
        CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
    {
        var nodeAddress = node.Address;
        var amount = Amount;
        var validatorInfo = await stakingContent.DelegateAsync(
            nodeAddress: nodeAddress,
            amount: amount,
            cancellationToken: cancellationToken);
        await Out.WriteLineAsJsonAsync(validatorInfo);
    }
}
