using System.ComponentModel.DataAnnotations;

namespace LibplanetConsole.Bank.DataAnnotations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class FungibleAssetValueAttribute : RegularExpressionAttribute
{
    public const string RegularExpression
        = @"(?<value>(?:\d{1,3}(,\d{3})*|\d+)(\.\d+)?)\s*(?<key>\w+)";

    public FungibleAssetValueAttribute()
        : base($"^{RegularExpression}$")
    {
    }
}
