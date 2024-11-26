using System.ComponentModel;
using System.Numerics;
using JSSoft.Commands;
using LibplanetConsole.Bank.DataAnnotations;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Node.Bank;

namespace LibplanetConsole.Node.Delegation.Commands;

[CommandSummary("Validator Commands.")]
[Category("Delegation")]
internal sealed class ValidatorCommand(Validator validator, IBank bank) : CommandMethodBase
{
    [CommandMethod]
    public async Task StakeAsync(long amount, CancellationToken cancellationToken)
    {
        await validator.StakeAsync(amount, cancellationToken);
    }

    [CommandMethod]
    public async Task PromoteAsync(long amount, CancellationToken cancellationToken)
    {
        await validator.PromoteAsync(amount, cancellationToken);
    }

    [CommandMethod]
    public async Task UnjailAsync(CancellationToken cancellationToken)
    {
        await validator.UnjailAsync(cancellationToken);
    }

    [CommandMethod]
    public async Task DelegateAsync(
        [FungibleAssetValue] string amount,
        CancellationToken cancellationToken)
    {
        var currencies = bank.Currencies;
        var amountValue = currencies.ToFungibleAssetValue(amount);
        await validator.DelegateAsync(amountValue, cancellationToken);
    }

    [CommandMethod]
    public async Task UndelegateAsync(
        string share,
        CancellationToken cancellationToken)
    {
        var shareValue = BigInteger.Parse(share);
        await validator.UndelegateAsync(shareValue, cancellationToken);
    }

    [CommandMethod]
    public async Task InfoAsync(CancellationToken cancellationToken)
    {
        var info = await validator.GetInfoAsync(cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }
}
