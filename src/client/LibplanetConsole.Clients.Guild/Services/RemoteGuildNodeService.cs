using System.ComponentModel.Composition;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Guild.Services;

namespace LibplanetConsole.Clients.Guild.Services;

[Export]
[Export(typeof(IRemoteService))]
internal sealed class RemoteGuildNodeService : RemoteService<IGuildNodeService>
{
}
