using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Console.Bank.Commands;

[CommandSummary("Display balance of specific address.")]
[Category("Bank")]
internal sealed partial class BalanceCommand(IApplication application)
    : CommandAsyncBase()
{
    [CommandProperty]
    public string Address { get; set; } = string.Empty;

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var addressable = application.GetAddressable(Address);
        if (addressable is IServiceProvider serviceProvider &&
            serviceProvider.GetService(typeof(IBank)) is IBank bank)
        {
            var address = addressable.Address;
            var balanceInfo = await bank.GetBalanceAsync(address, cancellationToken);
            await Out.WriteLineAsJsonAsync(balanceInfo);
        }
        else
        {
            throw new InvalidOperationException("Bank service is not available.");
        }
    }
}
