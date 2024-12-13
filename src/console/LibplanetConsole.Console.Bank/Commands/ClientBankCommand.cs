using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Console.Commands;
using LibplanetConsole.Console.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Bank.Commands;

internal sealed partial class ClientBankCommand(
    ClientCommand clientCommand, IClientCollection clients, ICurrencyCollection currencies)
    : CommandMethodBase(clientCommand, "bank")
{
    [CommandPropertyRequired(DefaultValue = "")]
    [CommandSummary("The address of the client. If not specified, the current client is used.")]
    [CommandPropertyCompletion(nameof(GetClientAddresses))]
    public static string Address { get; set; } = string.Empty;

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Display balance of specific address.")]
    [Category("Bank")]
    public async Task BalanceAsync(
        [CommandParameterCompletion(nameof(GetCurrencyAliases))]
        string currencyCode,
        CancellationToken cancellationToken)
    {
        var client = clients.GetClientOrCurrent(Address);
        var address = Address == string.Empty ? client.Address : new Address(Address);
        var bank = client.GetRequiredKeyedService<IClientBank>(IClient.Key);
        var currencyValue = currencies[currencyCode];
        var balance = await bank.GetBalanceAsync(address, currencyValue, cancellationToken);
        await Out.WriteLineAsync(currencies.ToString(balance));
    }

    [CommandMethod]
    public void Currency(
        [CommandParameterCompletion(nameof(GetCurrencyAliases))]
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

    private string[] GetClientAddresses()
        => [.. clients.Select(client => client.Address.ToString())];

    private string[] GetCurrencyAliases() => currencies.Codes;
}
