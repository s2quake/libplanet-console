namespace LibplanetConsole.Node;

public interface INodeContent
{
    string Name { get; }

    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
