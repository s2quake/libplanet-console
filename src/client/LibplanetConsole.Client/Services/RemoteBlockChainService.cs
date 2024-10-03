using LibplanetConsole.Common.Services;
using LibplanetConsole.Node.Services;

namespace LibplanetConsole.Client.Services;

internal sealed class RemoteBlockChainService(Client client)
    : RemoteService<IBlockChainService, IBlockChainCallback>(client)
{
}
