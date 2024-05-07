using System.Net;
using Libplanet.Crypto;
using Libplanet.Net;

namespace LibplanetConsole.Seeds;

public readonly record struct SeedNodeOptions
{
    public DnsEndPoint EndPoint { get; init; }

    public PrivateKey PrivateKey { get; init; }

    public AppProtocolVersion AppProtocolVersion { get; init; }
}
