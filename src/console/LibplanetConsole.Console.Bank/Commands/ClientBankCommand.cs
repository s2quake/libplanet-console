using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Bank;
using LibplanetConsole.Bank.DataAnnotations;
using LibplanetConsole.BlockChain;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Console.Commands;

namespace LibplanetConsole.Console.Bank.Commands;

[CommandSummary("Provides bank-related commands for the client.")]
[Category("Bank")]
internal sealed partial class ClientBankCommand(
    IServiceProvider serviceProvider,
    ClientCommand clientCommand,
    ICurrencyCollection currencies,
    IAddressCollection addresses)
    : ClientCommandMethodBase(serviceProvider, clientCommand, "bank")
{
    [CommandProperty]
    [CommandSummary("Specifies the address")]
    [CommandPropertyCompletion(nameof(GetAddresses))]
    public static Address Address { get; set; }

    [CommandMethod]
    [CommandSummary("Transfers the specified amount to the recipient address.")]
    [CommandMethodProperty(nameof(ClientAddress))]
    public async Task TransferAsync(
        [CommandParameterCompletion(nameof(GetAddresses))]
        Address recipientAddress,
        [FungibleAssetValue] string amount,
        CancellationToken cancellationToken = default)
    {
        var client = CurrentClient;
        var bank = client.GetRequiredKeyedService<IClientBank>(IClient.Key);
        var amountValue = currencies.ToFungibleAssetValue(amount);
        await bank.TransferAsync(
            recipientAddress, amountValue, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(ClientAddress))]
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Gets the balance of the specified currency for the client or " +
                    "specified address.")]
    public async Task BalanceAsync(
        [CommandParameterCompletion(nameof(GetCurrencyCodes))]
        string currencyCode,
        CancellationToken cancellationToken)
    {
        var client = CurrentClient;
        var address = Address == default ? client.Address : Address;
        var bank = client.GetRequiredKeyedService<IClientBank>(IClient.Key);
        var currency = currencies[currencyCode];
        var balance = await bank.GetBalanceAsync(address, currency, cancellationToken);
        await Out.WriteLineAsJsonAsync(currencies.ToString(balance), cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Displays information about a specific currency or lists all currency codes.")]
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
