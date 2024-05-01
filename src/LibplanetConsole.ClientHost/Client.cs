using System.ComponentModel.Composition;
using Libplanet.Crypto;
using LibplanetConsole.ClientServices;

namespace LibplanetConsole.ClientHost;

[Export]
[Export(typeof(IClient))]
[method: ImportingConstructor]
internal sealed class Client(
    ApplicationOptions options,
    [ImportMany] IEnumerable<IRemoteService> services)
    : ClientBase(GetPrivateKey(options), [.. services]), IClient
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
