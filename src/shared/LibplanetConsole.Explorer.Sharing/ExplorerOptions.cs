using System.Net;
using LibplanetConsole.Common;
using LibplanetConsole.Explorer.Serializations;

#if LIBPLANET_CONSOLE
namespace LibplanetConsole.Consoles.Explorer;
#elif LIBPLANET_NODE
namespace LibplanetConsole.Nodes.Explorer;
#elif LIBPLANET_CLIENT
namespace LibplanetConsole.Clients.Explorer;
#else
#error "Either LIBPLANET_CONSOLE or LIBPLANET_NODE must be defined."
#endif

public sealed record class ExplorerOptions
{
    public required EndPoint EndPoint { get; init; }

    public static implicit operator ExplorerOptions(ExplorerOptionsInfo info)
    {
        return new ExplorerOptions
        {
            EndPoint = EndPointUtility.Parse(info.EndPoint),
        };
    }

    public static implicit operator ExplorerOptionsInfo(ExplorerOptions explorerOptions)
    {
        return new ExplorerOptionsInfo
        {
            EndPoint = EndPointUtility.ToString(explorerOptions.EndPoint),
        };
    }
}
