using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Client.Delegation.Commands;

[CommandSummary("Delegate NCG to validator for delegation.")]
[Category("Delegation")]
internal sealed class DelegateCommand(IClient client, IDelegation delegation)
    : CommandAsyncBase()
{
    public override bool IsEnabled => client.IsRunning is true;

    [CommandPropertyRequired(DefaultValue = "")]
    public string NodeAddress { get; set; } = string.Empty;

    [CommandPropertyRequired(DefaultValue = 10)]
    public double Amount { get; set; }

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var nodeAddress = AddressUtility.ParseOrFallback(NodeAddress, client.NodeInfo.Address);
        var amount = Amount;
        var validatorInfo = await delegation.DelegateAsync(
            nodeAddress: nodeAddress,
            amount: amount,
            cancellationToken: cancellationToken);
        await Out.WriteLineAsJsonAsync(validatorInfo);
    }
}
