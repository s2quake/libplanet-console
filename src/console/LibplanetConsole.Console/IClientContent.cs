namespace LibplanetConsole.Console;

public interface IClientContent
{
    string Name { get; }

    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
