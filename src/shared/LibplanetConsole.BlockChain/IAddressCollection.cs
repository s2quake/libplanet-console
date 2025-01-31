using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using LibplanetConsole.Common.DataAnnotations;

namespace LibplanetConsole.BlockChain;

public interface IAddressCollection : IEnumerable<Address>
{
    string[] Aliases { get; }

    int Count { get; }

    Address this[int index] { get; }

    Address this[string alias] { get; }

    string this[Address address] { get; }

    void Add(AddressInfo addressInfo);

    void Add(string alias, Address address, params string[] tags)
        => Add(new() { Alias = alias, Address = address, Tags = tags });

    bool Remove(string alias);

    bool Contains(string alias);

    bool TryGetAddress(string alias, [MaybeNullWhen(false)] out Address address);

    bool TryGetAlias(Address address, [MaybeNullWhen(false)] out string alias);

    AddressInfo[] GetAddressInfos(params string[] tags);

    Address ToAddress(string text)
    {
        if (Regex.IsMatch(text, AddressAttribute.RegularExpression) is true)
        {
            return new Address(text);
        }
        else if (TryGetAddress(text, out Address address) is true)
        {
            return address;
        }
        else
        {
            throw new ArgumentException(
                message: $"'{text}' is not a valid address.",
                paramName: nameof(text));
        }
    }

    string[] GetAddresses(params string[] tags)
        => GetAddressInfos(tags).Select(addressInfo => addressInfo.Address.ToString()).ToArray();
}
