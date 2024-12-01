namespace LibplanetConsole.Node;

public interface INodeContent
{
    string Name { get; }

    IEnumerable<INodeContent> Dependencies { get; }

    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
