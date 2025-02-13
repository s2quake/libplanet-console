using System.ComponentModel;
using System.Security.Cryptography;
using GraphQL.AspNet.Attributes;

namespace LibplanetConsole.Node.Explorer.Types;

[GraphType("Currency")]
internal sealed class CurrencyValue
{
    [Description("The name of the currency.")]
    public string Ticker { get; init; } = string.Empty;

    [Description("The number of digits to treat as minor units (i.e., exponents).")]
    public byte DecimalPlaces { get; init; }

    [Description("The addresses who can mint this currency.  If this is null anyone can " +
                 "mint the currency.  On the other hand, unlike null, an empty set means no one " +
                 "can mint the currency.")]
    public Address[]? Minters { get; init; }

    [Description("The uppermost quantity of currency allowed to exist.  " +
                 "null means unlimited supply.")]
    public FungibleAssetValueValue? MaximumSupply { get; init; }

    [Description("Whether the total supply of this currency is trackable.")]
    public bool TotalSupplyTrackable { get; init; }

    [Description("The deterministic hash derived from other fields.")]
    public HashDigest<SHA256> Hash { get; init; }
}
