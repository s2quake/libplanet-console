using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Bank.DataAnnotations;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Console.Commands;
using LibplanetConsole.Console.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Bank.Commands;

internal sealed partial class ClientBankCommand(
    ClientCommand clientCommand,
    IClientCollection clients,
    ICurrencyCollection currencies,
    IAddressCollection addresses)
    : CommandMethodBase(clientCommand, "bank")
{
    [CommandProperty(InitValue = "")]
    [CommandSummary("The address of the client. If not specified, the current client is used.")]
    [CommandPropertyCompletion(nameof(GetClientAddresses))]
    public static string ClientAddress { get; set; } = string.Empty;

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
        var client = clients.GetClientOrCurrent(ClientAddress);
        var bank = client.GetRequiredKeyedService<IClientBank>(IClient.Key);
        var amountValue = currencies.ToFungibleAssetValue(amount);
        var recipientValue = addresses.ToAddress(recipientAddress);
        await bank.TransferAsync(
            recipientValue, amountValue, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(ClientAddress))]
    [CommandSummary("Display balance of specific address.")]
    [Category("Bank")]
    public async Task BalanceAsync(
        [CommandParameterCompletion(nameof(GetCurrencyCodes))]
        string currencyCode,
        CancellationToken cancellationToken)
    {
        var client = clients.GetClientOrCurrent(ClientAddress);
        var address = ClientAddress == string.Empty ? client.Address : addresses.ToAddress(Address);
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

    private string[] GetClientAddresses() =>
    [
        .. clients.Where(item => item.Alias != string.Empty).Select(item => item.Alias),
        .. clients.Select(item => item.Address.ToString())
    ];

    private string[] GetCurrencyCodes() => currencies.Codes;

    private string[] GetAddresses() => addresses.Aliases;
}
