using System.Security.Cryptography;
using System.Text;
using Libplanet.Crypto;

namespace LibplanetConsole.Common;

public static class AddressUtility
{
    public static Address Parse(string text) => new(text);

    public static string ToString(Address address) => $"{address}";

    public static string ToSafeString(Address? address) => $"{address}";

    public static Address Derive(Address address, byte[] key)
    {
        var bytes = address.ToByteArray();
        var hashed = GetHahsed(key, bytes);
        return new Address(hashed);

        static byte[] GetHahsed(byte[] key, byte[] bytes)
        {
            using var hmac = new HMACSHA1(key);
            return hmac.ComputeHash(bytes);
        }
    }

    public static Address Derive(Address address, string key)
        => Derive(address, Encoding.UTF8.GetBytes(key));
}
