using Libplanet.Crypto;

namespace LibplanetConsole.Common.Serializations;

public readonly record struct SeedInfo
{
    public static SeedInfo Empty { get; } = default;

    public GenesisOptionsInfo GenesisOptions { get; init; }

    public string BlocksyncSeedPeer { get; init; }

    public string ConsensusSeedPeer { get; init; }

    public SeedInfo Encrypt(PublicKey publicKey)
    {
        return this with
        {
            GenesisOptions = GenesisOptions.Encrypt(publicKey),
        };
    }

    public SeedInfo Decrypt(PrivateKey privateKey)
    {
        return this with
        {
            GenesisOptions = GenesisOptions.Decrypt(privateKey),
        };
    }
}
