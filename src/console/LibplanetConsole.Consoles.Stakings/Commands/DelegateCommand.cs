using System.ComponentModel;
using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Consoles.Extensions;

namespace LibplanetConsole.Consoles.Stakings.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Delegate NCG to validator for staking.")]
[Category("Staking")]
[method: ImportingConstructor]
internal sealed partial class DelegateCommand(IApplication application)
    : CommandAsyncBase()
{
    [CommandPropertyRequired(DefaultValue = "")]
    public string Address { get; set; } = string.Empty;

    [CommandPropertyRequired(DefaultValue = "")]
    public string NodeAddress { get; set; } = string.Empty;

    [CommandPropertyRequired(DefaultValue = 10)]
    public double Amount { get; set; }

    protected override async Task OnExecuteAsync(
        CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
    {
        var addressable = application.GetAddressable(Address);
        var node = application.GetViewingNode(Address, NodeAddress);
        if (addressable is IServiceProvider serviceProvider &&
            serviceProvider.GetService<IStakingContent>() is { } stakingContent)
        {
            var amount = Amount;
            var validatorInfo = await stakingContent.DelegateAsync(
                nodeAddress: node.Address,
                amount: amount,
                cancellationToken: cancellationToken);
            await Out.WriteLineAsJsonAsync(validatorInfo);
        }
        else
        {
            throw new InvalidOperationException("The application does not support staking.");
        }
    }
}
