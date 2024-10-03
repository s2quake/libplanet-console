using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Node.Services;

namespace LibplanetConsole.Client.Services;

[Export]
[Export(typeof(IRemoteService))]
internal sealed class RemoteBlockChainService(Client client)
    : RemoteService<IBlockChainService, IBlockChainCallback>(client)
{
}
