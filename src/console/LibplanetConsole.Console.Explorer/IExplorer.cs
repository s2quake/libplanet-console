using LibplanetConsole.Explorer;

namespace LibplanetConsole.Console.Explorer;

public interface IExplorer
{
    event EventHandler? Started;

    event EventHandler? Stopped;

    EndPoint EndPoint { get; set; }

    ExplorerInfo Info { get; }

    bool IsRunning { get; }

    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
