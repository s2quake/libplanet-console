using System.Globalization;
using System.Numerics;

namespace LibplanetConsole.Bank;

public static class AssetUtility
{
#pragma warning disable CS0618 // Type or member is obsolete
    public static Currency NCG { get; } = Currency.Legacy("NCG", 2, null);
#pragma warning restore CS0618 // Type or member is obsolete

    public static (BigInteger MajorUnit, BigInteger MinorUnit) GetValue(double amount)
    {
        var text = amount.ToString(CultureInfo.InvariantCulture);
        var items = text.Split('.');
        if (items.Length != 1 && items.Length != 2)
        {
            throw new InvalidOperationException($"Invalid amount.: '{text}'");
        }

        if (items.Length == 2 && items[1].Length > 2)
        {
            throw new ArgumentException(
                message: $"Only 2 decimal places are allowed.: '{text}'",
                paramName: nameof(amount));
        }

        var major = BigInteger.Parse(items[0]);
        var minorText = items.Length == 2 ? items[1].PadRight(2, '0') : "0";
        var minor = BigInteger.Parse(minorText);
        return (major, minor);
    }

    public static FungibleAssetValue GetNCG(double amount)
    {
        var (majorUnit, minorUnit) = GetValue(amount);
        return new FungibleAssetValue(NCG, majorUnit, minorUnit);
    }

    public static FungibleAssetValue GetValue(Currency currency, decimal amount)
        => FungibleAssetValue.Parse(currency, $"{amount:R}");

    public static void Verify(decimal amount, Currency currency)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(
                paramName: nameof(amount),
                message: "Amount must be greater than or equal to 0.");
        }

        if (Math.Round(amount, currency.DecimalPlaces) != amount)
        {
            throw new ArgumentException(
                paramName: nameof(amount),
                message: $"Amount must have up to {currency.DecimalPlaces} decimal places.");
        }
    }
}
