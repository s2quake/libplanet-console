#if LIBPLANET_NODE || LIBPLANET_CLIENT
using Grpc.Net.Client;
using static LibplanetConsole.Grpc.Console.ConsoleGrpcService;

namespace LibplanetConsole.Grpc.Console;

internal sealed class ConsoleService(GrpcChannel channel)
    : ConsoleGrpcServiceClient(channel)
{
}
#endif // LIBPLANET_NODE || LIBPLANET_CLIENT
