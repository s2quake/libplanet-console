using System.Collections;
using System.Diagnostics.CodeAnalysis;
using LibplanetConsole.Common;

namespace LibplanetConsole.Alias;

internal abstract class AliasCollectionBase : IEnumerable<AliasInfo>
{
    private readonly Dictionary<string, AliasInfo> _aliasInfoByAlias;

    protected AliasCollectionBase()
    {
        _aliasInfoByAlias = [];
    }

    public event EventHandler<AliasEventArgs>? Added;

    public event EventHandler<AliasUpdatedEventArgs>? Updated;

    public event EventHandler<AliasRemovedEventArgs>? Removed;

    public string[] Aliases => [.. _aliasInfoByAlias.Keys];

    public int Count => _aliasInfoByAlias.Count;

    public AliasInfo this[string alias] => _aliasInfoByAlias[alias];

    public string[] this[Address address]
    {
        get
        {
            var query = from item in _aliasInfoByAlias
                        where item.Value.Address == address
                        select item.Key;
            return [.. query];
        }
    }

    public void Add(AliasInfo aliasInfo)
    {
        var alias = aliasInfo.Alias;
        if (_aliasInfoByAlias.ContainsKey(alias))
        {
            throw new ArgumentException($"Alias '{alias}' already exists.");
        }

        _aliasInfoByAlias.Add(aliasInfo.Alias, aliasInfo);
        OnAdded(new AliasEventArgs(aliasInfo));
    }

    public void Update(AliasInfo aliasInfo)
    {
        var alias = aliasInfo.Alias;
        if (_aliasInfoByAlias.ContainsKey(alias) is false)
        {
            throw new KeyNotFoundException($"Alias '{alias}' not found.");
        }

        _aliasInfoByAlias[alias] = aliasInfo;
        OnUpdated(new AliasUpdatedEventArgs(alias, aliasInfo));
    }

    public bool Contains(string alias) => _aliasInfoByAlias.ContainsKey(alias);

    public bool Remove(string alias)
    {
        if (_aliasInfoByAlias.TryGetValue(alias, out var aliasInfo) is false)
        {
            return false;
        }

        var result = _aliasInfoByAlias.Remove(alias);
        OnRemoved(new AliasRemovedEventArgs(alias));
        return result;
    }

    public void Remove(Address address)
    {
        var query = from item in _aliasInfoByAlias
                    where item.Value.Address == address
                    select item.Key;
        foreach (var alias in query)
        {
            _aliasInfoByAlias.Remove(alias);
            OnRemoved(new AliasRemovedEventArgs(alias));
        }
    }

    public void Initialize(AliasInfo[] aliases)
    {
        foreach (var aliasInfo in aliases)
        {
            _aliasInfoByAlias.Add(aliasInfo.Alias, aliasInfo);
        }
    }

    public void Release()
    {
        _aliasInfoByAlias.Clear();
    }

    public void Load(string path)
    {
        if (Count > 0)
        {
            throw new InvalidOperationException("The collection is not empty.");
        }

        if (File.Exists(path) is false)
        {
            throw new ArgumentException($"File '{path}' not found.", nameof(path));
        }

        var json = File.ReadAllText(path);
        var aliases = JsonUtility.Deserialize<AliasInfo[]>(json);
        Initialize(aliases);
    }

    public void Save(string path)
    {
        var query = from aliasInfo in _aliasInfoByAlias.Values
                    where aliasInfo.Tags.Contains("temp") is false
                    select aliasInfo;
        var json = JsonUtility.Serialize(query.ToArray());
        File.WriteAllText(path, json);
    }

    public bool TryGetValue(string alias, [MaybeNullWhen(false)] out AliasInfo aliasInfo)
    {
        if (_aliasInfoByAlias.TryGetValue(alias, out var value) is true)
        {
            aliasInfo = value;
            return true;
        }

        aliasInfo = default;
        return false;
    }

    public AliasInfo[] GetAliasInfos(params string[] tags)
    {
        if (tags.Length == 0)
        {
            return [.. _aliasInfoByAlias.Values];
        }

        var query = from addressInfo in _aliasInfoByAlias.Values
                    where tags.Intersect(addressInfo.Tags).Any() is true
                    select addressInfo;
        return [.. query];
    }

    IEnumerator<AliasInfo> IEnumerable<AliasInfo>.GetEnumerator()
    {
        foreach (var addressInfo in _aliasInfoByAlias.Values)
        {
            yield return addressInfo;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        foreach (var addressInfo in _aliasInfoByAlias.Values)
        {
            yield return addressInfo;
        }
    }

    protected virtual void OnAdded(AliasEventArgs e)
    {
        Added?.Invoke(this, e);
    }

    protected virtual void OnRemoved(AliasRemovedEventArgs e)
    {
        Removed?.Invoke(this, e);
    }

    protected virtual void OnUpdated(AliasUpdatedEventArgs e)
    {
        Updated?.Invoke(this, e);
    }
}
