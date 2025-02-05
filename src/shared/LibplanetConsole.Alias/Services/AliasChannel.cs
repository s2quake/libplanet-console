#if LIBPLANET_NODE || LIBPLANET_CLIENT
using Grpc.Net.Client;
using LibplanetConsole.Common;

namespace LibplanetConsole.Alias.Services;

internal static class AliasChannel
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
#endif // LIBPLANET_NODE || LIBPLANET_CLIENT
