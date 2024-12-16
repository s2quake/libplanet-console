using System.ComponentModel;
using System.Numerics;
using JSSoft.Commands;
using LibplanetConsole.Bank.DataAnnotations;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Node.Bank;

namespace LibplanetConsole.Node.Delegation.Commands;

[CommandSummary("Delegation Commands.")]
[Category("Delegation")]
internal sealed class DelegationCommand(
    INode node, IDelegation delegation,
    ICurrencyCollection currencies) : CommandMethodBase
{
    [CommandMethod]
    public async Task StakeAsync(
        long ncg,
        CancellationToken cancellationToken)
    {
        await delegation.StakeAsync(ncg, cancellationToken);
    }

    [CommandMethod]
    public async Task PromoteAsync(
        [FungibleAssetValue] string guildGold,
        CancellationToken cancellationToken)
    {
        var value = currencies.ToFungibleAssetValue(guildGold);
        await delegation.PromoteAsync(value, cancellationToken);
    }

    [CommandMethod]
    public async Task UnjailAsync(CancellationToken cancellationToken)
    {
        await delegation.UnjailAsync(cancellationToken);
    }

    [CommandMethod]
    public async Task DelegateAsync(
        [FungibleAssetValue] string amount,
        CancellationToken cancellationToken)
    {
        var amountValue = currencies.ToFungibleAssetValue(amount);
        await delegation.DelegateAsync(amountValue, cancellationToken);
    }

    [CommandMethod]
    public async Task UndelegateAsync(
        string share,
        CancellationToken cancellationToken)
    {
        var shareValue = BigInteger.Parse(share, System.Globalization.NumberStyles.AllowThousands);
        await delegation.UndelegateAsync(shareValue, cancellationToken);
    }

    [CommandMethod]
    public async Task CommissionAsync(
        long commission,
        CancellationToken cancellationToken)
    {
        await delegation.SetCommissionAsync(commission, cancellationToken);
    }

    [CommandMethod]
    public async Task ClaimAsync(CancellationToken cancellationToken)
    {
        await delegation.ClaimAsync(cancellationToken);
    }

    [CommandMethod]
    public async Task InfoAsync(string address = "", CancellationToken cancellationToken = default)
    {
        var delegateeAddress = address == string.Empty ? node.Address : new Address(address);
        var info = await delegation.GetDelegateeInfoAsync(delegateeAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }
}
