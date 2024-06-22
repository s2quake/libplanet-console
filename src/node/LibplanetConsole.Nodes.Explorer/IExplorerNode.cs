using LibplanetConsole.Explorer;
using LibplanetConsole.Explorer.Serializations;

namespace LibplanetConsole.Nodes.Explorer;

public interface IExplorerNode
{
    event EventHandler? Started;

    event EventHandler? Stopped;

    ExplorerInfo Info { get; }

    bool IsRunning { get; }

    Task StartAsync(ExplorerOptions options, CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
