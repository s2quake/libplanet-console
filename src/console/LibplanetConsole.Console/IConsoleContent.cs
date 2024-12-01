namespace LibplanetConsole.Console;

public interface IConsoleContent
{
    string Name { get; }

    IEnumerable<IConsoleContent> Dependencies { get; }

    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
