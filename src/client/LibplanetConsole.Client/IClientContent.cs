namespace LibplanetConsole.Client;

public interface IClientContent
{
    string Name { get; }

    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
