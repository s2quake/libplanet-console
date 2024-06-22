using Libplanet.Crypto;
using LibplanetConsole.Common.Serializations;

namespace LibplanetConsole.Common;

public interface ISeedService
{
    Task<SeedInfo> GetSeedAsync(PublicKey publicKey, CancellationToken cancellationToken);

    Task<AppEndPoint> GetNodeEndPointAsync(CancellationToken cancellationToken);
}
