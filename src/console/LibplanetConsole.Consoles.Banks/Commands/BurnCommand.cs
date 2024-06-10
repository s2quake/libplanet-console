using System.ComponentModel;
using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Banks.Services;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Consoles.Banks.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Burn NCG from specific address.")]
[Category("Bank")]
[method: ImportingConstructor]
internal sealed partial class BurnCommand(IApplication application)
    : CommandAsyncBase()
{
    [CommandPropertyRequired]
    public double Amount { get; set; }

    [CommandProperty]
    public string Address { get; set; } = string.Empty;

    protected override async Task OnExecuteAsync(
        CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
    {
        var addressable = application.GetAddressable(Address);
        if (addressable is IServiceProvider serviceProvider &&
            serviceProvider.GetService(typeof(IBankContent)) is IBankContent bankContent)
        {
            var options = new BurnOptions
            {
                Amount = Amount,
            };
            var balanceInfo = await bankContent.BurnAsync(options, cancellationToken);
            await Out.WriteLineAsJsonAsync(balanceInfo);
        }
        else
        {
            throw new InvalidOperationException("Bank service is not available.");
        }
    }
}
