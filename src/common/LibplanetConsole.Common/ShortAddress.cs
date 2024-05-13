using Libplanet.Crypto;

namespace LibplanetConsole.Common;

public readonly struct ShortAddress(Address address)
{
    private readonly string _text = AddressUtility.ToString(address)[..8];

    public ShortAddress(PrivateKey privateKey)
        : this(privateKey.Address)
    {
    }

    public ShortAddress(PublicKey publicKey)
        : this(publicKey.Address)
    {
    }

    public static implicit operator string(ShortAddress shortAddress)
        => shortAddress._text;

    public static explicit operator ShortAddress(Address address)
        => new(address);

    public static explicit operator ShortAddress(PrivateKey privateKey)
        => new(privateKey);

    public static explicit operator ShortAddress(PublicKey publicKey)
        => new(publicKey);

    public override readonly string? ToString() => _text;
}
