using LibplanetConsole.Seed;

namespace LibplanetConsole.Console.Seed;

public interface ISeedService
{
    Task<SeedInfo> GetSeedAsync(PublicKey publicKey, CancellationToken cancellationToken);
}
