using System.ComponentModel;
using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Consoles;
using LibplanetConsole.Consoles.Extensions;

namespace LibplanetConsole.Consoles.Banks.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Mint NCG to specific address.")]
[Category("Bank")]
[method: ImportingConstructor]
internal sealed partial class MintCommand(IApplication application)
    : CommandAsyncBase()
{
    [CommandPropertyRequired]
    public string Address { get; set; } = string.Empty;

    [CommandPropertyRequired(DefaultValue = 10)]
    public double Amount { get; set; }

    protected override async Task OnExecuteAsync(
        CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
    {
        var addressable = application.GetAddressable(Address);
        if (addressable is IServiceProvider serviceProvider &&
            serviceProvider.GetService(typeof(IBankContent)) is IBankContent bankContent)
        {
            var amount = Amount;
            var balanceInfo = await bankContent.MintAsync(amount, cancellationToken);
            await Out.WriteLineAsJsonAsync(balanceInfo);
        }
        else
        {
            throw new InvalidOperationException("Bank service is not available.");
        }
    }
}
