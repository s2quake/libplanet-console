using System.ComponentModel.DataAnnotations;

namespace LibplanetConsole.Bank.DataAnnotations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class FungibleAssetValueAttribute : RegularExpressionAttribute
{
    public const string RegularExpression
        = @"(?<integer>\d{1,3}(,\d{3})*|\d+)(?<decimal>\.\d+)?\s*(?<code>\w+)";

    public FungibleAssetValueAttribute()
        : base($"^{RegularExpression}$")
    {
    }
}
