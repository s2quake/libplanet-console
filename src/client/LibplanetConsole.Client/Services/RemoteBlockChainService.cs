using System.ComponentModel.Composition;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Node.Services;

namespace LibplanetConsole.Client.Services;

[Export]
[Export(typeof(IRemoteService))]
[method: ImportingConstructor]
internal sealed class RemoteBlockChainService(Client client)
    : RemoteService<IBlockChainService, IBlockChainCallback>(client)
{
}
