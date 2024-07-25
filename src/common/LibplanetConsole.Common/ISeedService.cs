namespace LibplanetConsole.Common;

public interface ISeedService
{
    Task<SeedInfo> GetSeedAsync(AppPublicKey publicKey, CancellationToken cancellationToken);

    Task<AppEndPoint> GetNodeEndPointAsync(CancellationToken cancellationToken);
}
