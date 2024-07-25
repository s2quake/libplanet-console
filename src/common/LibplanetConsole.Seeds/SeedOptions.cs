using LibplanetConsole.Common;

namespace LibplanetConsole.Seeds;

internal record class SeedOptions
{
    public required AppPrivateKey PrivateKey { get; init; }

    public required AppEndPoint EndPoint { get; init; }

    public TimeSpan RefreshInterval { get; init; } = TimeSpan.FromSeconds(5);

    public TimeSpan PeerLifetime { get; init; } = TimeSpan.FromSeconds(120);

    public TimeSpan PingTimeout { get; init; } = TimeSpan.FromSeconds(5);
}
