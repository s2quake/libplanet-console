using System.Security;
using System.Text;
using System.Text.Json;
using Bencodex;
using Bencodex.Types;
using Libplanet.Crypto;

namespace LibplanetConsole.Common.Extensions;

public static class PrivateKeyExtensions
{
    private static readonly Codec _codec = new();

    public static SecureString ToSecureString(this PrivateKey @this)
    {
        var secureString = new SecureString();
        var text = ByteUtil.Hex(@this.ByteArray);
        secureString.AppendString(text);
        return secureString;
    }

    public static byte[] Sign(this PrivateKey @this, object obj)
    {
        if (obj is IValue value)
        {
            return @this.Sign(_codec.Encode(value));
        }

        if (obj is IBencodable bencodable)
        {
            return @this.Sign(_codec.Encode(bencodable.Bencoded));
        }

        var json = JsonSerializer.Serialize(obj);
        var bytes = Encoding.UTF8.GetBytes(json);
        return @this.Sign(bytes);
    }
}
