using Libplanet.Net;
using LibplanetConsole.Common;

namespace LibplanetConsole.Seeds;

internal record class SeedOptions
{
    public SeedOptions(
        AppPrivateKey privateKey, AppEndPoint endPoint, AppProtocolVersion appProtocolVersion)
    {
        PrivateKey = privateKey;
        EndPoint = endPoint;
        AppProtocolVersion = appProtocolVersion;
    }

    public AppPrivateKey PrivateKey { get; }

    public AppEndPoint EndPoint { get; }

    public AppProtocolVersion AppProtocolVersion { get; }

    public TimeSpan RefreshInterval { get; init; } = TimeSpan.FromSeconds(5);

    public TimeSpan PeerLifetime { get; init; } = TimeSpan.FromSeconds(120);

    public TimeSpan PingTimeout { get; init; } = TimeSpan.FromSeconds(5);
}
