using System.ComponentModel;
using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Clients;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Stakings.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Undelegate Share from validator.")]
[Category("Staking")]
[method: ImportingConstructor]
internal sealed class UndelegateCommand(IClient client, IStakingClient stakingContent)
    : CommandAsyncBase()
{
    public override bool IsEnabled => client.IsRunning is true;

    [CommandPropertyRequired(DefaultValue = "")]
    public string NodeAddress { get; set; } = string.Empty;

    [CommandPropertyRequired(DefaultValue = 500)]
    public long ShareAmount { get; set; }

    protected override async Task OnExecuteAsync(
        CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
    {
        var nodeAddress = NodeAddress != string.Empty ?
           AddressUtility.Parse(NodeAddress) :
           client.NodeInfo.Address;
        var shareAmount = ShareAmount;
        var validatorInfo = await stakingContent.UndelegateAsync(
            nodeAddress: nodeAddress,
            shareAmount: shareAmount,
            cancellationToken: cancellationToken);
        await Out.WriteLineAsJsonAsync(validatorInfo);
    }
}
