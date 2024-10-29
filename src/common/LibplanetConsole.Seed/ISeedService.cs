namespace LibplanetConsole.Seed;

public interface ISeedService
{
    bool IsEnabled { get; }

    Task<SeedInfo> GetSeedAsync(PublicKey publicKey, CancellationToken cancellationToken);
}
