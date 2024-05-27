using System.Net;
using LibplanetConsole.Clients.Serializations;
using LibplanetConsole.Common;

#if LIBPLANET_CONSOLE
namespace LibplanetConsole.Consoles;
#elif LIBPLANET_NODE
namespace LibplanetConsole.Nodes;
#elif LIBPLANET_CLIENT
namespace LibplanetConsole.Clients;
#else
#error "Either LIBPLANET_CONSOLE or LIBPLANET_NODE must be defined."
#endif

public sealed record class ClientOptions
{
    public static ClientOptions Default { get; } = new ClientOptions();

    public EndPoint NodeEndPoint { get; init; } = DnsEndPointUtility.Parse("0.0.0.0:0");

    public static implicit operator ClientOptions(ClientOptionsInfo info)
    {
        return new ClientOptions
        {
            NodeEndPoint = EndPointUtility.Parse(info.NodeEndPoint),
        };
    }

    public static implicit operator ClientOptionsInfo(ClientOptions clientOptions)
    {
        return new ClientOptionsInfo
        {
            NodeEndPoint = EndPointUtility.ToString(clientOptions.NodeEndPoint),
        };
    }
}
