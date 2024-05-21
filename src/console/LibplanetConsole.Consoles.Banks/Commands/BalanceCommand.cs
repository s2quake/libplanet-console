using System.ComponentModel;
using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Consoles;
using LibplanetConsole.Consoles.Extensions;

namespace LibplanetConsole.Consoles.Banks.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Display balance of specific address.")]
[Category("Bank")]
[method: ImportingConstructor]
internal sealed partial class BalanceCommand(IApplication application)
    : CommandAsyncBase()
{
    [CommandPropertyRequired]
    public string Address { get; set; } = string.Empty;

    protected override async Task OnExecuteAsync(
        CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
    {
        var addressable = application.GetAddressable(Address);
        if (addressable is IServiceProvider serviceProvider &&
            serviceProvider.GetService(typeof(IBankContent)) is IBankContent bankContent)
        {
            var address = addressable.Address;
            var balanceInfo = await bankContent.GetBalanceAsync(address, cancellationToken);
            await Out.WriteLineAsJsonAsync(balanceInfo);
        }
        else
        {
            throw new InvalidOperationException("Bank service is not available.");
        }
    }
}
