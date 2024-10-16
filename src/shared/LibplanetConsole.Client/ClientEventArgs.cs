#if LIBPLANET_CLIENT
namespace LibplanetConsole.Client;
#elif LIBPLANET_CONSOLE
namespace LibplanetConsole.Console;
#else
#error LIBPLANET_CLIENT, or LIBPLANET_CONSOLE must be defined.
#endif

public sealed class ClientEventArgs(ClientInfo clientInfo) : EventArgs
{
    public ClientInfo ClientInfo { get; } = clientInfo;
}
