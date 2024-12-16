namespace LibplanetConsole.Node;

public interface INodeContent
{
    string Name { get; }

    IEnumerable<INodeContent> Dependencies { get; }

    int Order => 0;

    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
