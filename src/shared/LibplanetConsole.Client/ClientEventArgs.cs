#if LIBPLANET_CLIENT || LIBPLANET_CONSOLE
namespace LibplanetConsole.Client;

public sealed class ClientEventArgs(ClientInfo clientInfo) : EventArgs
{
    public ClientInfo ClientInfo { get; } = clientInfo;
}
#endif // LIBPLANET_CLIENT || LIBPLANET_CONSOLE
