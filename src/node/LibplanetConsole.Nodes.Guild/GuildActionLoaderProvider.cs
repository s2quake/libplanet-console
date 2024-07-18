using System.ComponentModel.Composition;
using Libplanet.Action.Loader;
using LibplanetConsole.Common.Actions;
using Nekoyume.Module.Guild;

namespace LibplanetConsole.Nodes.Guild;

[Export(typeof(IActionLoaderProvider))]
internal sealed class GuildActionLoaderProvider : IActionLoaderProvider
{
    public IActionLoader GetActionLoader()
        => new AssemblyActionLoader(typeof(GuildModule).Assembly);
}
