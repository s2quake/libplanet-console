namespace LibplanetConsole.Seed;

public sealed record class SeedOptions
{
    public required PrivateKey PrivateKey { get; init; }

    public required int Port { get; init; }

    public TimeSpan RefreshInterval { get; init; } = TimeSpan.FromSeconds(5);

    public TimeSpan PeerLifetime { get; init; } = TimeSpan.FromSeconds(120);

    public TimeSpan PingTimeout { get; init; } = TimeSpan.FromSeconds(5);
}
