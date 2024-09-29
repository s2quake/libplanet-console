using Bencodex.Types;
using Libplanet.Crypto;

namespace LibplanetConsole.Common;

public readonly partial record struct AppAddress
{
    public static AppAddress FromBencodex(IValue value)
        => new AppAddress(new Address(value));

    public IValue ToBencodex() => _address.Bencoded;
}
