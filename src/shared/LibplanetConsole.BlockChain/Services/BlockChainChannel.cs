#if LIBPLANET_CLIENT || LIBPLANET_CONSOLE
using Grpc.Net.Client;

namespace LibplanetConsole.BlockChain.Services;

internal static class BlockChainChannel
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
#endif // LIBPLANET_CONSOLE || LIBPLANET_CLIENT
