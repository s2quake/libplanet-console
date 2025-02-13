using System.ComponentModel.DataAnnotations;

namespace LibplanetConsole.Node.Explorer.Types;

internal sealed class FungibleAssetValueValue(FungibleAssetValue fav)
{
    [Required]
    public required decimal Quantity { get; init; }

    [Required]
    public required string Ticker { get; init; } = fav.Currency.Ticker;

    [Required]
    public required byte DecimalPlaces { get; init; }

    public Address[]? Minters { get; init; }
}
