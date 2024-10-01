using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Bank.Services;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Console.Bank.Commands;

[CommandSummary("Mint NCG to specific address.")]
[Category("Bank")]
internal sealed partial class MintCommand(IApplication application)
    : CommandAsyncBase()
{
    [CommandPropertyRequired(DefaultValue = 10)]
    public decimal Amount { get; set; }

    [CommandProperty]
    public string Address { get; set; } = string.Empty;

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var addressable = application.GetAddressable(Address);
        if (addressable is IServiceProvider serviceProvider &&
            serviceProvider.GetService(typeof(IBank)) is IBank bank)
        {
            var options = new MintOptions
            {
                Amount = Amount,
            };
            var balanceInfo = await bank.MintAsync(options, cancellationToken);
            await Out.WriteLineAsJsonAsync(balanceInfo);
        }
        else
        {
            throw new InvalidOperationException("Bank service is not available.");
        }
    }
}
