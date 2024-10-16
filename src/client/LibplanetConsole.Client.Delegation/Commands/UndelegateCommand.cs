using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Client.Delegation.Commands;

[CommandSummary("Undelegate Share from validator.")]
[Category("Delegation")]
internal sealed class UndelegateCommand(IClient client, IDelegation delegation)
    : CommandAsyncBase()
{
    public override bool IsEnabled => client.IsRunning is true;

    [CommandPropertyRequired(DefaultValue = "")]
    public string NodeAddress { get; set; } = string.Empty;

    [CommandPropertyRequired(DefaultValue = 500)]
    public long ShareAmount { get; set; }

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var nodeAddress = AddressUtility.ParseOrFallback(NodeAddress, client.NodeInfo.Address);
        var shareAmount = ShareAmount;
        var validatorInfo = await delegation.UndelegateAsync(
            nodeAddress: nodeAddress,
            shareAmount: shareAmount,
            cancellationToken: cancellationToken);
        await Out.WriteLineAsJsonAsync(validatorInfo);
    }
}
