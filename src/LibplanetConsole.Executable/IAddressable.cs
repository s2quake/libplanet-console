using Libplanet.Crypto;

namespace LibplanetConsole.Executable;

public interface IAddressable
{
    Address Address { get; }
}
