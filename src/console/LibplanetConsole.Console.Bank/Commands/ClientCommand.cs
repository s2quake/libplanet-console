using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Bank.Services;
using LibplanetConsole.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Bank.Commands;

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
        decimal amount,
        CancellationToken cancellationToken)
    {
        var address = Address;
        var client = application.GetClient(address);
        var bank = client.GetRequiredService<IBank>();
        var options = new MintOptions
        {
            Amount = amount,
        };
        var balanceInfo = await bank.MintAsync(options, cancellationToken);
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
        decimal amount,
        CancellationToken cancellationToken)
    {
        var client = application.GetClient(Address);
        var targetAddressable = application.GetAddressable(targetAddress);
        var bank = client.GetRequiredService<IBank>();
        var options = new TransferOptions
        {
            TargetAddress = targetAddressable.Address,
            Amount = amount,
        };
        var balanceInfo = await bank.TransferAsync(options, cancellationToken);
        await Out.WriteLineAsJsonAsync(balanceInfo);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Burn NCG.")]
    [Category("Bank")]
    public async Task BurnAsync(
        [CommandSummary("The amount of NCG to burn.")]
        decimal amount,
        CancellationToken cancellationToken)
    {
        var client = application.GetClient(Address);
        var bank = client.GetRequiredService<IBank>();
        var options = new BurnOptions
        {
            Amount = amount,
        };
        var balanceInfo = await bank.BurnAsync(options, cancellationToken);
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
        var bank = client.GetRequiredService<IBank>();
        var balanceInfo = await bank.GetBalanceAsync(client.Address, cancellationToken);
        await Out.WriteLineAsJsonAsync(balanceInfo);
    }
}
