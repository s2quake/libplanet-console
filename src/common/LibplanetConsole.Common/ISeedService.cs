namespace LibplanetConsole.Common;

public interface ISeedService
{
    Task<SeedInfo> GetSeedAsync(AppPublicKey publicKey, CancellationToken cancellationToken);
}
