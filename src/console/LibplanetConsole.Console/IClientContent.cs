namespace LibplanetConsole.Console;

public interface IClientContent
{
    string Name { get; }

    IEnumerable<IClientContent> Dependencies { get; }

    int Order => 0;

    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
