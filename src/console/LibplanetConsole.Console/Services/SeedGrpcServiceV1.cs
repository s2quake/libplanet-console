using Grpc.Core;
using LibplanetConsole.Grpc.Seed;
using LibplanetConsole.Seed;

namespace LibplanetConsole.Console.Services;

public sealed class SeedGrpcServiceV1(ISeedService seedService)
    : SeedGrpcService.SeedGrpcServiceBase
{
    public async override Task<GetSeedResponse> GetSeed(
        GetSeedRequest request, ServerCallContext context)
    {
        var publicKey = new PrivateKey().PublicKey;
        var seedInfo = await seedService.GetSeedAsync(publicKey, context.CancellationToken);
        return new GetSeedResponse
        {
            SeedResult = seedInfo,
        };
    }
}
