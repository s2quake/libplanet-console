using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using LibplanetConsole.Common.DataAnnotations;

namespace LibplanetConsole.Alias;

public interface IAliasCollection : IEnumerable<AliasInfo>
{
    string[] Aliases { get; }

    int Count { get; }

    AliasInfo this[string alias] { get; }

    string this[Address address] { get; }

    bool Contains(string alias);

    bool TryGetValue(string alias, [MaybeNullWhen(false)] out AliasInfo aliasInfo);

    bool TryGetAlias(Address address, [MaybeNullWhen(false)] out string alias);

    AliasInfo[] GetAliasInfos(params string[] tags);

    Address ToAddress(string text)
    {
        if (Regex.IsMatch(text, AddressAttribute.RegularExpression) is true)
        {
            return new Address(text);
        }
        else if (TryGetValue(text, out AliasInfo aliasInfo) is true)
        {
            return aliasInfo.Address;
        }
        else
        {
            throw new ArgumentException(
                message: $"'{text}' is not a valid address.",
                paramName: nameof(text));
        }
    }

    string[] GetAddresses(params string[] tags)
        => [.. GetAliasInfos(tags).Select(addressInfo => addressInfo.Address.ToString())];
}
