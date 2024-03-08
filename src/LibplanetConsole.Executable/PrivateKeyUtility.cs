using System.Security.Cryptography;
using System.Text;
using Libplanet.Crypto;

namespace LibplanetConsole.Executable;

static class PrivateKeyUtility
{
    public static PrivateKey Create(string name)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(name));
        return new(bytes);
    }
}
