namespace LibplanetConsole.Seed;

public interface ISeedService
{
    Task<SeedInfo> GetSeedAsync(PublicKey publicKey, CancellationToken cancellationToken);
}
