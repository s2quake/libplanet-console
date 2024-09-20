namespace LibplanetConsole.Common;

public readonly record struct SeedInfo
{
    public static SeedInfo Empty { get; } = default;

    public AppPeer BlocksyncSeedPeer { get; init; }

    public AppPeer ConsensusSeedPeer { get; init; }

    public SeedInfo Encrypt(AppPublicKey publicKey) => this with
    {
    };

    public SeedInfo Decrypt(AppPrivateKey privateKey) => this with
    {
    };
}
