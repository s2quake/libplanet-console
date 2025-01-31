#if LIBPLANET_NODE
using Grpc.Net.Client;
using static LibplanetConsole.Seed.Grpc.SeedGrpcService;

namespace LibplanetConsole.Seed.Services;

internal sealed class SeedService(GrpcChannel channel)
    : SeedGrpcServiceClient(channel)
{
}
#endif // LIBPLANET_NODE
