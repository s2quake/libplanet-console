using System.Security.Cryptography;
using System.Text;
using Libplanet.Crypto;

namespace LibplanetConsole.Common;

public static class AddressUtility
{
    public static Address Parse(string text) => new(text);

    public static string ToString(Address address) => $"{address}";

    public static Address Derive(Address address, byte[] key)
    {
        var bytes = address.ToByteArray();
        byte[] hashed;

        using (var hmac = new HMACSHA1(key))
        {
            hashed = hmac.ComputeHash(bytes);
        }

        return new Address(hashed);
    }

    public static Address Derive(Address address, string key) =>
        Derive(address, Encoding.UTF8.GetBytes(key));
}
