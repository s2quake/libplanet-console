using System.ComponentModel;
using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Consoles.Extensions;
using LibplanetConsole.Stakings.Services;

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
        var node = addressable.GetNode();
        if (addressable is IServiceProvider serviceProvider &&
            serviceProvider.GetService<IStakingContent>() is { } stakingContent)
        {
            var options = new DelegateOptions
            {
                NodeAddress = node.Address,
                Amount = Amount,
            };
            var validatorInfo = await stakingContent.DelegateAsync(options, cancellationToken);
            await Out.WriteLineAsJsonAsync(validatorInfo);
        }
        else
        {
            throw new InvalidOperationException("The application does not support staking.");
        }
    }
}
