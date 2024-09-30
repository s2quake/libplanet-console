using System.Security.Cryptography;
using System.Text;

namespace LibplanetConsole.Common.Extensions;

public static class AddressExtensions
{
    public static Address Derive(this Address @this, byte[] key)
    {
        var bytes = @this.ToByteArray();
        var hashed = GetHahsed(key, bytes);
        return new Address(hashed);

        static byte[] GetHahsed(byte[] key, byte[] bytes)
        {
            using var hmac = new HMACSHA1(key);
            return hmac.ComputeHash(bytes);
        }
    }

    public static Address Derive(this Address @this, string key)
        => Derive(@this, Encoding.UTF8.GetBytes(key));
}
