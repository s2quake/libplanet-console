using System.Security.Cryptography;
using System.Text;
using Libplanet.Common;
using Libplanet.Crypto;

namespace LibplanetConsole.Common;

public static class PrivateKeyUtility
{
    public static PrivateKey Create(string name)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(name));
        return new(bytes);
    }

    public static PrivateKey Parse(string text) => new(text);

    public static string ToString(PrivateKey privateKey) =>
        ByteUtil.Hex(privateKey.ByteArray);
}
