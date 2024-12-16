namespace LibplanetConsole.Console;

public interface IConsoleContent
{
    string Name { get; }

    IEnumerable<IConsoleContent> Dependencies { get; }

    int Order => 0;

    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
