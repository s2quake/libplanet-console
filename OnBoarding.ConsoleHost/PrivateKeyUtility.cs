using System.Security.Cryptography;
using System.Text;
using Libplanet.Crypto;

namespace OnBoarding.ConsoleHost;

static class PrivateKeyUtility
{
    public static PrivateKey Create(string name)
    {
        var bytes = ComputeHash(Encoding.UTF8.GetBytes(name));
        return new(bytes);

        static byte[] ComputeHash(byte[] bytes)
        {
            using var sha = SHA256.Create();
            return sha.ComputeHash(bytes);
        }
    }
}
