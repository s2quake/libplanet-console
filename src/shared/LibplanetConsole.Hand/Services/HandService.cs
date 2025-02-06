#if LIBPLANET_CLIENT || LIBPLANET_CONSOLE
using Grpc.Net.Client;
using static LibplanetConsole.Hand.Grpc.HandGrpcService;

namespace LibplanetConsole.Hand.Services;

internal sealed class HandService(GrpcChannel channel)
    : HandGrpcServiceClient(channel)
{
}
#endif // LIBPLANET_CLIENT || LIBPLANET_CONSOLE
