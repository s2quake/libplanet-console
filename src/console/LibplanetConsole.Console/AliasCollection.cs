using LibplanetConsole.Alias;

namespace LibplanetConsole.Console;

internal sealed class AliasCollection : AliasCollectionBase, IAliasCollection
{
    private readonly string _localPath;
    private readonly SynchronizationContext _synchronizationContext;
    private readonly ILogger<AliasCollection> _logger;

    public AliasCollection(
        IApplicationOptions options,
        SynchronizationContext synchronizationContext,
        ILogger<AliasCollection> logger)
    {
        Load(options.AliasPath);
        _localPath = options.AliasPath;
        _synchronizationContext = synchronizationContext;
        _logger = logger;
    }

    public Task AddAsync(AliasInfo aliasInfo, CancellationToken cancellationToken)
    {
        void Action() => _synchronizationContext.Send(_ => Add(aliasInfo), null);
        return Task.Run(Action, cancellationToken);
    }

    public Task RemoveAsync(string alias, CancellationToken cancellationToken)
    {
        void Action() => _synchronizationContext.Send(_ => Remove(alias), null);
        return Task.Run(Action, cancellationToken);
    }

    public Task UpdateAsync(AliasInfo aliasInfo, CancellationToken cancellationToken)
    {
        void Action() => _synchronizationContext.Send(_ => Update(aliasInfo), null);
        return Task.Run(Action, cancellationToken);
    }

    protected override void OnAdded(AliasEventArgs e)
    {
        base.OnAdded(e);
        Save(_localPath);
    }

    protected override void OnRemoved(AliasRemovedEventArgs e)
    {
        base.OnRemoved(e);
        Save(_localPath);
    }

    protected override void OnUpdated(AliasUpdatedEventArgs e)
    {
        base.OnUpdated(e);
        Save(_localPath);
    }

    private new void Save(string path)
    {
        if (path != string.Empty)
        {
            try
            {
                base.Save(path);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to save aliases.");
            }
        }
    }

    private new void Load(string path)
    {
        if (File.Exists(path) is true)
        {
            try
            {
                base.Load(path);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to load aliases.");
            }
        }
    }
}
