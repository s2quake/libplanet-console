using System.ComponentModel;
using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Consoles.Extensions;

namespace LibplanetConsole.Consoles.Stakings.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Promote validator with NCG.")]
[Category("Staking")]
[method: ImportingConstructor]
internal sealed class PromoteCommand(IApplication application)
    : CommandAsyncBase()
{
    [CommandPropertyRequired]
    public string NodeAddress { get; set; } = string.Empty;

    [CommandProperty("amount", 'a', InitValue = 10)]
    public double Amount { get; set; }

    protected override async Task OnExecuteAsync(
        CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
    {
        var node = application.GetNode(NodeAddress);
        if (node.GetService(typeof(IStakingContent)) is IStakingContent stakingContent)
        {
            var amount = Amount;
            var validatorInfo = await stakingContent.PromoteAsync(amount, cancellationToken);
            await Out.WriteLineAsJsonAsync(validatorInfo);
        }
        else
        {
            throw new InvalidOperationException("Staking service is not available.");
        }
    }
}
