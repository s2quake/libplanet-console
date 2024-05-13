using LibplanetConsole.Common.Serializations;

namespace LibplanetConsole.Common;

public interface ISeedService
{
    Task<SeedInfo> GetSeedAsync(CancellationToken cancellationToken);

    Task<string> GetNodeEndPointAsync(CancellationToken cancellationToken);
}
