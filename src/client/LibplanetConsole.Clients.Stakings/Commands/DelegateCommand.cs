using System.ComponentModel;
using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Clients;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Clients.Stakings.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Delegate NCG to validator for staking.")]
[Category("Staking")]
[method: ImportingConstructor]
internal sealed class DelegateCommand(IClient client, IStakingClient stakingContent)
    : CommandAsyncBase()
{
    public override bool IsEnabled => client.IsRunning is true;

    [CommandPropertyRequired(DefaultValue = "")]
    public string NodeAddress { get; set; } = string.Empty;

    [CommandPropertyRequired(DefaultValue = 10)]
    public double Amount { get; set; }

    protected override async Task OnExecuteAsync(
        CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
    {
        var nodeAddress = NodeAddress != string.Empty ?
            AddressUtility.Parse(NodeAddress) :
            client.NodeInfo.Address;
        var amount = Amount;
        var validatorInfo = await stakingContent.DelegateAsync(
            nodeAddress: nodeAddress,
            amount: amount,
            cancellationToken: cancellationToken);
        await Out.WriteLineAsJsonAsync(validatorInfo);
    }
}
