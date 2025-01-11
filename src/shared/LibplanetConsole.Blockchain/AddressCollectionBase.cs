using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

#if LIBPLANET_NODE
namespace LibplanetConsole.Node;
#elif LIBPLANET_CLIENT
namespace LibplanetConsole.Client;
#elif LIBPLANET_CONSOLE
namespace LibplanetConsole.Console;
#else
#error LIBPLANET_NODE, LIBPLANET_CLIENT, or LIBPLANET_CONSOLE must be defined.
#endif

internal abstract class AddressCollectionBase : IAddressCollection
{
    private readonly OrderedDictionary _addressInfoByAlias;
    private readonly Dictionary<Address, string> _aliasByAddress;

    protected AddressCollectionBase()
    {
        _addressInfoByAlias = [];
        _aliasByAddress = [];
    }

    protected AddressCollectionBase(IEnumerable<AddressInfo> addressInfos)
    {
        _addressInfoByAlias = new OrderedDictionary(addressInfos.Count());
        foreach (var address in addressInfos)
        {
            _addressInfoByAlias.Add(address.Alias, address.Address);
        }

        _aliasByAddress = addressInfos.ToDictionary(item => item.Address, item => item.Alias);
    }

    public string[] Aliases => [.. _aliasByAddress.Values];

    public int Count => _addressInfoByAlias.Count;

    public Address this[int index]
    {
        get
        {
            if (_addressInfoByAlias[index] is not AddressInfo addressInfo)
            {
                throw new UnreachableException("Cannot happen.");
            }

            return addressInfo.Address;
        }
    }

    public Address this[string alias]
    {
        get
        {
            if (_addressInfoByAlias[alias] is not AddressInfo addressInfo)
            {
                throw new KeyNotFoundException("No such address.");
            }

            return addressInfo.Address;
        }
    }

    public string this[Address address]
    {
        get
        {
            if (_aliasByAddress.TryGetValue(address, out var alias) is false)
            {
                throw new KeyNotFoundException("No such address.");
            }

            return alias;
        }
    }

    public void Add(AddressInfo addressInfo)
    {
        _addressInfoByAlias.Add(addressInfo.Alias, addressInfo);
        _aliasByAddress.Add(addressInfo.Address, addressInfo.Alias);
    }

    public bool Contains(string alias) => _addressInfoByAlias.Contains(alias);

    public bool Remove(string alias)
    {
        if (_addressInfoByAlias[alias] is not AddressInfo addressInfo)
        {
            return false;
        }

        _addressInfoByAlias.Remove(alias);
        _aliasByAddress.Remove(addressInfo.Address);
        return true;
    }

    public void RemoteAt(int index)
    {
        if (_addressInfoByAlias[index] is not AddressInfo addressInfo)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        _addressInfoByAlias.RemoveAt(index);
        _aliasByAddress.Remove(addressInfo.Address);
    }

    public void Clear()
    {
        _addressInfoByAlias.Clear();
        _aliasByAddress.Clear();
    }

    public bool TryGetAddress(string alias, [MaybeNullWhen(false)] out Address address)
    {
        if (_addressInfoByAlias[alias] is AddressInfo value)
        {
            address = value.Address;
            return true;
        }

        address = default;
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

    public AddressInfo[] GetAddressInfos(params string[] tags)
    {
        if (tags.Length == 0)
        {
            return [.. _addressInfoByAlias.Values.OfType<AddressInfo>()];
        }

        var query = from addressInfo in _addressInfoByAlias.Values.OfType<AddressInfo>()
                    where tags.Intersect(addressInfo.Tags).Any() is true
                    select addressInfo;
        return [.. query];
    }

    IEnumerator<Address> IEnumerable<Address>.GetEnumerator()
    {
        foreach (var addressInfo in _addressInfoByAlias.Values.OfType<AddressInfo>())
        {
            yield return addressInfo.Address;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        foreach (var addressInfo in _addressInfoByAlias.Values.OfType<AddressInfo>())
        {
            yield return addressInfo.Address;
        }
    }
}
