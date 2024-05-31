using System.ComponentModel;
using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Clients;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Clients.Stakings.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Display validator information.")]
[Category("Staking")]
[method: ImportingConstructor]
internal sealed class ValidatorCommand(IClient client, IStakingClient stakingContent)
    : CommandAsyncBase()
{
    public override bool IsEnabled => client.IsRunning is true;

    [CommandPropertyRequired(DefaultValue = new string[] { })]
    public string[] Validator { get; set; } = [];

    protected override async Task OnExecuteAsync(
        CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
    {
        var validatorInfos = await stakingContent.GetValidatorsAsync(cancellationToken);
        await Out.WriteLineAsJsonAsync(validatorInfos);
    }
}
