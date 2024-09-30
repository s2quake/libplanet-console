using Libplanet.Crypto;

namespace LibplanetConsole.Console;

public interface IAddressable
{
    Address Address { get; }
}
