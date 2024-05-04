using System.Net;
using LibplanetConsole.Common;

namespace LibplanetConsole.ClientServices;

public record class ClientOptions
{
    public static ClientOptions Default { get; } = new ClientOptions();

    public EndPoint NodeEndPoint { get; init; } = DnsEndPointUtility.GetEndPoint("0.0.0.0:0");
}
