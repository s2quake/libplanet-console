namespace LibplanetConsole.Client;

public interface IClientContent
{
    string Name { get; }

    IEnumerable<IClientContent> Dependencies { get; }

    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
