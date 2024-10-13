namespace LibplanetConsole.Console;

public interface INodeContent
{
    string Name { get; }

    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
