using System.ComponentModel.Composition;
using Libplanet.Crypto;
using LibplanetConsole.ClientHost.Services;
using LibplanetConsole.Clients;

namespace LibplanetConsole.ClientHost;

[Export]
[Export(typeof(IClient))]
[method: ImportingConstructor]
internal sealed class Client(ApplicationOptions options, RemoteNodeContext remoteNodeContext)
    : ClientBase(GetPrivateKey(options), remoteNodeContext), IClient
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
