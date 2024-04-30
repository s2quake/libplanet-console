using Libplanet.Crypto;

namespace LibplanetConsole.Executable;

public interface IIdentifier
{
    PrivateKey PrivateKey { get; }

    Address Address => PrivateKey.Address;
}
