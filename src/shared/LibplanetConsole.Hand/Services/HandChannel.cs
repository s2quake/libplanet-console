#if LIBPLANET_CLIENT || LIBPLANET_CONSOLE
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using LibplanetConsole.Common;

namespace LibplanetConsole.Seed.Services;

internal static class SeedChannel
{
    private static readonly GrpcChannelOptions _channelOptions = new()
    {
    };

    public static GrpcChannel CreateChannel(Uri url)
    {
        var address = url.ToString();
        return GrpcChannel.ForAddress(address, _channelOptions);
    }
}
#endif // LIBPLANET_CLIENT || LIBPLANET_CONSOLE
