using System.ComponentModel;
using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Banks.Services;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Consoles.Banks.Commands;

[Export(typeof(ICommand))]
[PartialCommand]
[method: ImportingConstructor]
internal sealed partial class ClientCommand(IApplication application) : CommandMethodBase
{
    [CommandPropertyRequired(DefaultValue = "")]
    [CommandSummary("The address of the client. If not specified, the current client is used.")]
    public static string Address { get; set; } = string.Empty;

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Mint NCG to specific address.")]
    [Category("Bank")]
    public async Task MintAsync(
        [CommandSummary("The amount of NCG to mint.")]
        double amount,
        CancellationToken cancellationToken)
    {
        var address = Address;
        var client = application.GetClient(address);
        var bankContent = client.GetService<IBankContent>();
        var options = new MintOptions
        {
            Amount = amount,
        };
        var balanceInfo = await bankContent.MintAsync(options, cancellationToken);
        await Out.WriteLineAsJsonAsync(balanceInfo);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Transfer NCG to specific address.")]
    [Category("Bank")]
    public async Task TransferAsync(
        [CommandSummary("The address of the recipient.")]
        string targetAddress,
        [CommandSummary("The amount of NCG to transfer.")]
        double amount,
        CancellationToken cancellationToken)
    {
        var client = application.GetClient(Address);
        var targetAddressable = application.GetAddressable(targetAddress);
        var bankContent = client.GetService<IBankContent>();
        var options = new TransferOptions
        {
            TargetAddress = targetAddressable.Address,
            Amount = amount,
        };
        var balanceInfo = await bankContent.TransferAsync(options, cancellationToken);
        await Out.WriteLineAsJsonAsync(balanceInfo);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Burn NCG.")]
    [Category("Bank")]
    public async Task BurnAsync(
        [CommandSummary("The amount of NCG to burn.")]
        double amount,
        CancellationToken cancellationToken)
    {
        var client = application.GetClient(Address);
        var bankContent = client.GetService<IBankContent>();
        var options = new BurnOptions
        {
            Amount = amount,
        };
        var balanceInfo = await bankContent.BurnAsync(options, cancellationToken);
        await Out.WriteLineAsJsonAsync(balanceInfo);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Display balance of specific address.")]
    [Category("Bank")]
    public async Task BalanceAsync(CancellationToken cancellationToken)
    {
        var address = Address;
        var client = application.GetClient(address);
        var bankContent = client.GetService<IBankContent>();
        var balanceInfo = await bankContent.GetBalanceAsync(client.Address, cancellationToken);
        await Out.WriteLineAsJsonAsync(balanceInfo);
    }
}
