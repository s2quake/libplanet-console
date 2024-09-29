using Bencodex.Types;
using Libplanet.Crypto;

namespace LibplanetConsole.Common;

public readonly partial record struct AppPublicKey
{
    public static AppPublicKey FromBencodex(IValue value)
    {
        if (value is not Binary binary)
        {
            throw new InvalidCastException("The given value is not a binary.");
        }

        return new(new PublicKey(binary.ByteArray));
    }

    public IValue ToBencodex() => new Binary(_publicKey.Format(true));
}
