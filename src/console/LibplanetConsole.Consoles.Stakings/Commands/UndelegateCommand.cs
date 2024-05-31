using System.ComponentModel;
using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Consoles.Extensions;
using LibplanetConsole.Stakings.Services;

namespace LibplanetConsole.Consoles.Stakings.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Undelegate Share from validator.")]
[Category("Staking")]
[method: ImportingConstructor]
internal sealed class UndelegateCommand(IApplication application)
    : CommandAsyncBase()
{
    [CommandPropertyRequired(DefaultValue = "")]
    public string Address { get; set; } = string.Empty;

    [CommandPropertyRequired(DefaultValue = "")]
    public string NodeAddress { get; set; } = string.Empty;

    [CommandPropertyRequired(DefaultValue = 500)]
    public long ShareAmount { get; set; }

    protected override async Task OnExecuteAsync(
        CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
    {
        var addressable = application.GetAddressable(Address);
        var node = addressable.GetNode();
        if (addressable is IServiceProvider serviceProvider &&
            serviceProvider.GetService<IStakingContent>() is { } stakingContent)
        {
            var options = new UndelegateOptions
            {
                NodeAddress = node.Address,
                ShareAmount = ShareAmount,
            };
            var validatorInfo = await stakingContent.UndelegateAsync(options, cancellationToken);
            await Out.WriteLineAsJsonAsync(validatorInfo);
        }
        else
        {
            throw new InvalidOperationException("Staking service is not available.");
        }
    }
}
