using LibplanetConsole.Explorer;

namespace LibplanetConsole.Node.Explorer;

public interface IExplorer
{
    event EventHandler? Started;

    event EventHandler? Stopped;

    ExplorerInfo Info { get; }

    bool IsRunning { get; }

    Task<ExplorerInfo> StartAsync(ExplorerOptions options, CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
