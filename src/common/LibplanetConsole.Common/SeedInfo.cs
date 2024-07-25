namespace LibplanetConsole.Common;

public readonly record struct SeedInfo
{
    public static SeedInfo Empty { get; } = default;

    public GenesisInfo GenesisInfo { get; init; }

    public AppPeer BlocksyncSeedPeer { get; init; }

    public AppPeer ConsensusSeedPeer { get; init; }

    public SeedInfo Encrypt(AppPublicKey publicKey) => this with
    {
        GenesisInfo = GenesisInfo.Encrypt(publicKey),
    };

    public SeedInfo Decrypt(AppPrivateKey privateKey) => this with
    {
        GenesisInfo = GenesisInfo.Decrypt(privateKey),
    };
}
