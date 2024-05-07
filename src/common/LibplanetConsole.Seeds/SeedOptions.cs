using System.Net;
using Libplanet.Crypto;
using Libplanet.Net;

namespace LibplanetConsole.Seeds;

internal record class SeedOptions
{
    public SeedOptions(
        PrivateKey privateKey, DnsEndPoint endPoint, AppProtocolVersion appProtocolVersion)
    {
        PrivateKey = privateKey;
        EndPoint = endPoint;
        AppProtocolVersion = appProtocolVersion;
    }

    public PrivateKey PrivateKey { get; }

    public DnsEndPoint EndPoint { get; }

    public AppProtocolVersion AppProtocolVersion { get; }

    public TimeSpan RefreshInterval { get; init; } = TimeSpan.FromSeconds(5);

    public TimeSpan PeerLifetime { get; init; } = TimeSpan.FromSeconds(120);

    public TimeSpan PingTimeout { get; init; } = TimeSpan.FromSeconds(5);
}
