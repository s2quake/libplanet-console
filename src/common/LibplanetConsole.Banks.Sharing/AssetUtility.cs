using System.Globalization;
using System.Numerics;
using Libplanet.Types.Assets;
using Nekoyume.Action.DPoS.Misc;

namespace LibplanetConsole.Banks;

public static class AssetUtility
{
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
        return new FungibleAssetValue(Asset.GovernanceToken, majorUnit, minorUnit);
    }
}
