using System.ComponentModel.DataAnnotations;

namespace LibplanetConsole.Bank.DataAnnotations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class FungibleAssetValueAttribute : RegularExpressionAttribute
{
    public const string RegularExpression = @"(?<value>\d+(\.\d+)?)(?<key>\w+)";

    public FungibleAssetValueAttribute()
        : base($"^{RegularExpression}$")
    {
    }
}
