using System.ComponentModel;
using System.Numerics;
using JSSoft.Commands;
using LibplanetConsole.Bank;
using LibplanetConsole.Bank.DataAnnotations;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Node.Delegation.Commands;

[CommandSummary("Delegation Commands.")]
[Category("Delegation")]
internal sealed class DelegationCommand(
    INode node,
    IDelegation delegation,
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
    public async Task SlashAsync(
        long slashFactor = 10, CancellationToken cancellationToken = default)
    {
        await delegation.SlashAsync(slashFactor, cancellationToken);
    }

    [CommandMethod]
    public async Task DelegateeInfoAsync(
        Address address = default,
        CancellationToken cancellationToken = default)
    {
        var actualAddress = address == default ? node.Address : address;
        var info = await delegation.GetDelegateeInfoAsync(actualAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }

    [CommandMethod]
    public async Task DelegatorInfoAsync(
        Address address = default,
        CancellationToken cancellationToken = default)
    {
        var actualAddress = address == default ? node.Address : address;
        var info = await delegation.GetDelegatorInfoAsync(actualAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }

    [CommandMethod]
    public async Task StakeInfoAsync(
        Address address = default, CancellationToken cancellationToken = default)
    {
        var actualAddress = address == default ? node.Address : address;
        var info = await delegation.GetStakeInfoAsync(actualAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }
}
