using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Bank.DataAnnotations;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Console.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Bank.Commands;

internal sealed partial class ClientBankCommand(
    IServiceProvider serviceProvider,
    ClientCommand clientCommand,
    ICurrencyCollection currencies,
    IAddressCollection addresses)
    : ClientCommandMethodBase(serviceProvider, clientCommand, "bank")
{
    [CommandProperty(InitValue = "")]
    [CommandSummary("Specifies the address")]
    [CommandPropertyCompletion(nameof(GetAddresses))]
    public static string Address { get; set; } = string.Empty;

    [CommandMethod]
    [CommandMethodProperty(nameof(ClientAddress))]
    public async Task TransferAsync(
        [CommandParameterCompletion(nameof(GetAddresses))]
        string recipientAddress,
        [FungibleAssetValue] string amount,
        CancellationToken cancellationToken = default)
    {
        var client = GetClientOrCurrent(ClientAddress);
        var bank = client.GetRequiredKeyedService<IClientBank>(IClient.Key);
        var amountValue = currencies.ToFungibleAssetValue(amount);
        var recipientValue = addresses.ToAddress(recipientAddress);
        await bank.TransferAsync(
            recipientValue, amountValue, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(ClientAddress))]
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Display balance of specific address.")]
    [Category("Bank")]
    public async Task BalanceAsync(
        [CommandParameterCompletion(nameof(GetCurrencyCodes))]
        string currencyCode,
        CancellationToken cancellationToken)
    {
        var client = GetClientOrCurrent(ClientAddress);
        var address = Address == string.Empty ? client.Address : addresses.ToAddress(Address);
        var bank = client.GetRequiredKeyedService<IClientBank>(IClient.Key);
        var currency = currencies[currencyCode];
        var balance = await bank.GetBalanceAsync(address, currency, cancellationToken);
        await Out.WriteLineAsJsonAsync(currencies.ToString(balance), cancellationToken);
    }

    [CommandMethod]
    public void Currency(
        [CommandParameterCompletion(nameof(GetCurrencyCodes))]
        string code = "")
    {
        if (code == string.Empty)
        {
            var currencyAliases = currencies.Codes;
            Out.WriteLineAsJson(currencyAliases);
        }
        else
        {
            var currency = currencies[code];
            Out.WriteLineAsJson(currency);
        }
    }

    private string[] GetCurrencyCodes() => currencies.Codes;

    private string[] GetAddresses() => addresses.Aliases;
}
