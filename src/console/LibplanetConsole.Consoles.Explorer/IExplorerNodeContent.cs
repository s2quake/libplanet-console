using LibplanetConsole.Common;
using LibplanetConsole.Explorer;

namespace LibplanetConsole.Consoles.Explorer;

public interface IExplorerNodeContent
{
    event EventHandler? Started;

    event EventHandler? Stopped;

    AppEndPoint EndPoint { get; set; }

    ExplorerInfo Info { get; }

    bool IsRunning { get; }

    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
