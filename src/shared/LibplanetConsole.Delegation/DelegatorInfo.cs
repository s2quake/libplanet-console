using LibplanetConsole.Grpc.Delegation;

#if LIBPLANET_NODE
namespace LibplanetConsole.Node.Delegation;
#elif LIBPLANET_CLIENT
namespace LibplanetConsole.Client.Delegation;
#elif LIBPLANET_CONSOLE
namespace LibplanetConsole.Console.Delegation;
#else
#error LIBPLANET_NODE, LIBPLANET_CLIENT, or LIBPLANET_CONSOLE must be defined.
#endif

public readonly record struct DelegatorInfo(
    long LastDistributeHeight,
    string Share)
{
    public static implicit operator DelegatorInfo(DelegatorInfoProto delegationInfo) => new()
    {
        LastDistributeHeight = delegationInfo.LastDistributeHeight,
        Share = delegationInfo.Share,
    };

    public static implicit operator DelegatorInfoProto(DelegatorInfo delegationInfo) => new()
    {
        LastDistributeHeight = delegationInfo.LastDistributeHeight,
        Share = delegationInfo.Share,
    };
}
