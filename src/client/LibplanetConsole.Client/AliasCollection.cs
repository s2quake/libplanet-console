using LibplanetConsole.Alias;

namespace LibplanetConsole.Client;

internal sealed class AliasCollection : AliasCollectionBase, IClientContent
{
    public AliasCollection()
    {
    }

    IEnumerable<IClientContent> IClientContent.Dependencies { get; } = [];

    string IClientContent.Name => "aliases";

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
