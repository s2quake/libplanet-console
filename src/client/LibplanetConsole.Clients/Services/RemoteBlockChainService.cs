using System.ComponentModel.Composition;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Nodes.Services;

namespace LibplanetConsole.Clients.Services;

[Export]
[Export(typeof(IRemoteService))]
internal sealed class RemoteBlockChainService
    : RemoteService<IBlockChainService>
{
}
