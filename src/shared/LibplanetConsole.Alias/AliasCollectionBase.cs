using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace LibplanetConsole.Alias;

internal abstract class AliasCollectionBase : IAliasCollection
{
    private readonly Dictionary<string, AliasInfo> _aliasInfoByAlias;
    private readonly Dictionary<Address, string> _aliasByAddress;

    protected AliasCollectionBase()
    {
        _aliasInfoByAlias = [];
        _aliasByAddress = [];
    }

    protected AliasCollectionBase(IEnumerable<AliasInfo> addressInfos)
    {
        _aliasInfoByAlias = new Dictionary<string, AliasInfo>(addressInfos.Count());
        foreach (var aliasInfo in addressInfos)
        {
            _aliasInfoByAlias.Add(aliasInfo.Alias, aliasInfo);
        }

        _aliasByAddress = addressInfos.ToDictionary(item => item.Address, item => item.Alias);
    }

    public event EventHandler<AliasEventArgs>? Added;

    public event EventHandler<AliasUpdatedEventArgs>? Updated;

    public event EventHandler<AliasRemovedEventArgs>? Removed;

    public string[] Aliases => [.. _aliasByAddress.Values];

    public int Count => _aliasInfoByAlias.Count;

    public AliasInfo this[string alias] => _aliasInfoByAlias[alias];

    public string this[Address address] => _aliasByAddress[address];

    public void Add(AliasInfo aliasInfo)
    {
        if (_aliasInfoByAlias.ContainsKey(aliasInfo.Alias))
        {
            throw new ArgumentException($"Alias '{aliasInfo.Alias}' already exists.");
        }

        if (_aliasByAddress.ContainsKey(aliasInfo.Address))
        {
            throw new ArgumentException($"Address '{aliasInfo.Address}' already exists.");
        }

        _aliasInfoByAlias.Add(aliasInfo.Alias, aliasInfo);
        _aliasByAddress.Add(aliasInfo.Address, aliasInfo.Alias);
        Added?.Invoke(this, new AliasEventArgs(aliasInfo));
    }

    public void Update(string alias, AliasInfo aliasInfo)
    {
        if (_aliasInfoByAlias.TryGetValue(alias, out var oldAliasInfo) is false)
        {
            throw new KeyNotFoundException($"Alias '{alias}' not found.");
        }

        if (_aliasInfoByAlias.TryGetValue(aliasInfo.Alias, out var newAliasInfo)
            && newAliasInfo != oldAliasInfo)
        {
            throw new ArgumentException($"Alias '{aliasInfo.Alias}' already exists.");
        }

        _aliasByAddress.Remove(oldAliasInfo.Address);
        _aliasInfoByAlias.Remove(alias);
        _aliasInfoByAlias.Add(aliasInfo.Alias, aliasInfo);
        _aliasByAddress.Add(aliasInfo.Address, aliasInfo.Alias);
        Updated?.Invoke(this, new AliasUpdatedEventArgs(alias, aliasInfo));
    }

    public bool Contains(string alias) => _aliasInfoByAlias.ContainsKey(alias);

    public bool Remove(string alias)
    {
        if (_aliasInfoByAlias.TryGetValue(alias, out var aliasInfo) is false)
        {
            return false;
        }

        _aliasInfoByAlias.Remove(alias);
        _aliasByAddress.Remove(aliasInfo.Address);
        Removed?.Invoke(this, new AliasRemovedEventArgs(alias));
        return true;
    }

    public bool Remove(Address address)
    {
        if (_aliasByAddress.TryGetValue(address, out var alias) is false)
        {
            return false;
        }

        _aliasByAddress.Remove(address);
        _aliasInfoByAlias.Remove(alias);
        Removed?.Invoke(this, new AliasRemovedEventArgs(alias));
        return true;
    }

    public void Clear()
    {
        _aliasInfoByAlias.Clear();
        _aliasByAddress.Clear();
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

    public bool TryGetAlias(Address address, [MaybeNullWhen(false)] out string alias)
    {
        if (_aliasByAddress.TryGetValue(address, out alias) is false)
        {
            alias = default;
            return false;
        }

        return true;
    }

    public AliasInfo[] GetAliasInfos(params string[] tags)
    {
        if (tags.Length == 0)
        {
            return [.. _aliasInfoByAlias.Values.OfType<AliasInfo>()];
        }

        var query = from addressInfo in _aliasInfoByAlias.Values.OfType<AliasInfo>()
                    where tags.Intersect(addressInfo.Tags).Any() is true
                    select addressInfo;
        return [.. query];
    }

    IEnumerator<AliasInfo> IEnumerable<AliasInfo>.GetEnumerator()
    {
        foreach (var addressInfo in _aliasInfoByAlias.Values.OfType<AliasInfo>())
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
}
