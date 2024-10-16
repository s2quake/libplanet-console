using LibplanetConsole.Common.Services;
using LibplanetConsole.Guild.Services;

namespace LibplanetConsole.Client.Guild.Services;

[Export]
[Export(typeof(IRemoteService))]
internal sealed class RemoteGuildNodeService : RemoteService<IGuildService>
{
}
