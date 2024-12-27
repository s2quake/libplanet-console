using LibplanetConsole.Grpc.Guild;
using static LibplanetConsole.Grpc.TypeUtility;

#if LIBPLANET_NODE
namespace LibplanetConsole.Node;
#elif LIBPLANET_CLIENT
namespace LibplanetConsole.Client;
#elif LIBPLANET_CONSOLE
namespace LibplanetConsole.Console;
#else
#error LIBPLANET_NODE, LIBPLANET_CLIENT, or LIBPLANET_CONSOLE must be defined.
#endif

public readonly record struct GuildInfo
{
    public Address Address { get; init; }

    public Address ValidatorAddress { get; init; }

    public Address GuildMasterAddress { get; init; }

    public static implicit operator GuildInfo(GuildInfoProto guildInfo) => new()
    {
        Address = ToAddress(guildInfo.Address),
        ValidatorAddress = ToAddress(guildInfo.ValidatorAddress),
        GuildMasterAddress = ToAddress(guildInfo.GuildMasterAddress),
    };

    public static implicit operator GuildInfoProto(GuildInfo guildInfo) => new()
    {
        Address = ToGrpc(guildInfo.Address),
        ValidatorAddress = ToGrpc(guildInfo.ValidatorAddress),
        GuildMasterAddress = ToGrpc(guildInfo.GuildMasterAddress),
    };
}
