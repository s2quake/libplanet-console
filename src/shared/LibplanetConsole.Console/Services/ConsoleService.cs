#if LIBPLANET_NODE || LIBPLANET_CLIENT
using Grpc.Net.Client;
using static LibplanetConsole.Console.Grpc.ConsoleGrpcService;

namespace LibplanetConsole.Console.Services;

internal sealed class ConsoleService(GrpcChannel channel)
    : ConsoleGrpcServiceClient(channel)
{
}
#endif // LIBPLANET_NODE || LIBPLANET_CLIENT
