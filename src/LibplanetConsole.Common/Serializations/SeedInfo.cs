namespace LibplanetConsole.Common.Serializations;

public record struct SeedInfo
{
    public static SeedInfo Empty { get; } = default;

    public GenesisOptionsInfo GenesisOptions { get; set; }

    public string BlocksyncSeedPeer { get; set; }

    public string ConsensusSeedPeer { get; set; }
}
