using Libplanet.Crypto;
using LibplanetConsole.ClientServices;

namespace LibplanetConsole.ClientHost;

internal sealed class Client(ApplicationOptions options)
    : ClientBase(GetPrivateKey(options)), IClient
{
    public static PrivateKey GetPrivateKey(ApplicationOptions options)
    {
        if (options.PrivateKey == string.Empty)
        {
            return new PrivateKey();
        }

        return new PrivateKey(options.PrivateKey);
    }
}
