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
    private readonly OrderedDictionary _addressByAlias;
    private readonly Dictionary<Address, string> _aliasByAddress;

    protected AddressCollectionBase()
    {
        _addressByAlias = [];
        _aliasByAddress = [];
    }

    protected AddressCollectionBase(IEnumerable<AddressInfo> addresses)
    {
        _addressByAlias = new OrderedDictionary(addresses.Count());
        foreach (var address in addresses)
        {
            _addressByAlias.Add(address.Alias, address.Address);
        }

        _aliasByAddress = addresses.ToDictionary(item => item.Address, item => item.Alias);
    }

    public string[] Aliases => [.. _aliasByAddress.Values];

    public int Count => _addressByAlias.Count;

    public Address this[int index]
        => (Address)(_addressByAlias[index] ?? throw new UnreachableException("Cannot happen."));

    public Address this[string alias]
    {
        get
        {
            if (_addressByAlias[alias] is not { } address)
            {
                throw new KeyNotFoundException("No such address.");
            }

            return (Address)address;
        }
    }

    public void Add(string alias, Address address)
    {
        _addressByAlias.Add(alias, address);
        _aliasByAddress.Add(address, alias);
    }

    public bool Contains(string alias) => _addressByAlias.Contains(alias);

    public bool Remove(string alias)
    {
        if (_addressByAlias[alias] is not Address currency)
        {
            return false;
        }

        _addressByAlias.Remove(alias);
        _aliasByAddress.Remove(currency);
        return true;
    }

    public void RemoteAt(int index)
    {
        if (_addressByAlias[index] is not Address currency)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        _addressByAlias.RemoveAt(index);
        _aliasByAddress.Remove(currency);
    }

    public void Clear()
    {
        _addressByAlias.Clear();
        _aliasByAddress.Clear();
    }

    public bool TryGetAddress(string alias, [MaybeNullWhen(false)] out Address address)
    {
        if (_addressByAlias[alias] is { } value)
        {
            address = (Address)value;
            return true;
        }

        address = default;
        return false;
    }

    public string GetAlias(Address address)
    {
        if (_aliasByAddress.TryGetValue(address, out var alias) is false)
        {
            throw new KeyNotFoundException("Not supported currency.");
        }

        return alias;
    }

    public AddressInfo[] GetAddressInfos() => [.. _aliasByAddress.Select(GetAddressInfo)];

    IEnumerator<Address> IEnumerable<Address>.GetEnumerator()
        => _addressByAlias.Values.OfType<Address>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _addressByAlias.Values.GetEnumerator();

    private static AddressInfo GetAddressInfo(KeyValuePair<Address, string> item)
        => new() { Alias = item.Value, Address = item.Key };
}
