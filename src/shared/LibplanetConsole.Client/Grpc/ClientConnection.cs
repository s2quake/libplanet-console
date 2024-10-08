#if LIBPLANET_CONSOLE
using LibplanetConsole.Grpc;

namespace LibplanetConsole.Client.Grpc;

internal sealed class ClientConnection(ClientService clientService)
    : ConnectionMonitor<ClientService>(clientService, FuncAsync)
{
    private static async Task FuncAsync(
        ClientService clientService, CancellationToken cancellationToken)
    {
        await clientService.PingAsync(new(), cancellationToken: cancellationToken);
    }
}
#endif // LIBPLANET_CONSOLE || LIBPLANET_CLIENT
