#if LIBPLANET_NODE || LIBPLANET_CLIENT
using Grpc.Net.Client;
using static LibplanetConsole.Grpc.Console.ConsoleGrpcService;

#if LIBPLANET_NODE
namespace LibplanetConsole.Node.Services;
#elif LIBPLANET_CLIENT
namespace LibplanetConsole.Client.Services;
#endif

internal sealed class ConsoleService(GrpcChannel channel)
    : ConsoleGrpcServiceClient(channel)
{
}
#endif // LIBPLANET_NODE || LIBPLANET_CLIENT
