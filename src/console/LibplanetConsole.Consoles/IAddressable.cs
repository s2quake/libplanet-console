using Libplanet.Crypto;

namespace LibplanetConsole.Consoles;

public interface IAddressable
{
    Address Address { get; }
}
