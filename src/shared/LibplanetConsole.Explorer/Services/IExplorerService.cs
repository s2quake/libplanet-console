namespace LibplanetConsole.Explorer.Services;

public interface IExplorerService
{
    Task<ExplorerInfo> StartAsync(ExplorerOptions options, CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);

    Task<ExplorerInfo> GetInfoAsync(CancellationToken cancellationToken);
}
