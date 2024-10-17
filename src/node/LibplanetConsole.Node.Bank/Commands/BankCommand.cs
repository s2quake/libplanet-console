using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using JSSoft.Commands;
using Lib9c;
using LibplanetConsole.Bank.DataAnnotations;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Node.Bank.Commands;

[CommandSummary("Bank Commands.")]
[Category("Bank")]
internal sealed class BankCommand(Bank bank) : CommandMethodBase
{
    private readonly Dictionary<string, Currency> _currencyByCode = new()
    {
        { "gg", Currencies.GuildGold },
    };

    [CommandMethod]
    public async Task MintAsync(
        Address address,
        [FungibleAssetValue]
        string amount,
        CancellationToken cancellationToken)
    {
        var match = Regex.Match(amount, @"(?<value>\d+(\.\d+)?)(?<key>\w+)");
        var key = match.Groups["key"].Value;
        var value = match.Groups["value"].Value;
        var fav = FungibleAssetValue.Parse(Currencies.GuildGold, value);
        var balance = await bank.MintAsync(address, fav, cancellationToken);
        await Out.WriteLineAsJsonAsync(balance);
    }

    [CommandMethod]
    public async Task BalanceAsync(
        Address address, CancellationToken cancellationToken)
    {
        var balance = await bank.GetBalanceAsync(address, Currencies.GuildGold, cancellationToken);
        await Out.WriteLineAsJsonAsync(balance);
    }

    // [CommandMethod]
    // public async Task CurrenciesAsync(CancellationToken cancellationToken)
    // {
    //     var balance = await bank.GetBalanceAsync(address, Currencies.GuildGold, cancellationToken);
    //     await Out.WriteLineAsJsonAsync(balance);
    // }
}
