using JSSoft.Communication;
using LibplanetConsole.Common.Serializations;

namespace LibplanetConsole.Common;

public interface ISeedService
{
    [ServerMethod]
    Task<SeedInfo> GetSeedAsync(CancellationToken cancellationToken);

    [ServerMethod]
    Task<string> GetNodeEndPointAsync(CancellationToken cancellationToken);
}
