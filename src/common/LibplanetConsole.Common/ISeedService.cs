using Libplanet.Crypto;

namespace LibplanetConsole.Common;

public interface ISeedService
{
    Task<SeedInfo> GetSeedAsync(PublicKey publicKey, CancellationToken cancellationToken);
}
