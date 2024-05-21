using System.ComponentModel;
using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Consoles;
using LibplanetConsole.Stakings;

namespace LibplanetConsole.Stakins.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Display validator information.")]
[Category("Staking")]
[method: ImportingConstructor]
internal sealed partial class ValidatorCommand(INodeCollection nodes)
    : CommandAsyncBase()
{
    [CommandPropertyRequired(DefaultValue = new string[] { })]
    public string[] Validator { get; set; } = [];

    protected override async Task OnExecuteAsync(
        CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
    {
        var node = nodes.Current ?? throw new InvalidOperationException("Node is not set.");
        if (node.GetService(typeof(IStakingContent)) is IStakingContent staking)
        {
            var validatorInfos = await staking.GetValidatorsAsync(cancellationToken);
            await Out.WriteLineAsJsonAsync(validatorInfos);
        }
        else
        {
            throw new InvalidOperationException("Staking service is not available.");
        }
    }
}
