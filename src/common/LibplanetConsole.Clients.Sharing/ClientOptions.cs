using System.Net;
using LibplanetConsole.Common;

namespace LibplanetConsole.Clients;

public record class ClientOptions
{
    public static ClientOptions Default { get; } = new ClientOptions();

    public EndPoint NodeEndPoint { get; init; } = DnsEndPointUtility.Parse("0.0.0.0:0");
}
