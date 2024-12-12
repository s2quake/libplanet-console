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

public interface IAddressCollection : IEnumerable<Address>
{
    string[] Aliases { get; }

    int Count { get; }

    Address this[int index] { get; }

    Address this[string alias] { get; }

    bool Contains(string alias);

    bool TryGetAddress(string alias, [MaybeNullWhen(false)] out Address address);

    string GetAlias(Address address);
}
