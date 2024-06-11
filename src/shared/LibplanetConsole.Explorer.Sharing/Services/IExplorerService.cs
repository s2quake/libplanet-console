using LibplanetConsole.Explorer.Serializations;

namespace LibplanetConsole.Explorer.Services;

public interface IExplorerService
{
    Task<ExplorerInfo> StartAsync(ExplorerOptionsInfo options, CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);

    Task<ExplorerInfo> GetInfoAsync(CancellationToken cancellationToken);
}
