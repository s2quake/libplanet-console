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
    string Share,
    StakeInfo StakeInfo)
{
    public static implicit operator DelegatorInfo(DelegatorInfoProto delegationInfo)
    {
        return new DelegatorInfo
        {
            LastDistributeHeight = delegationInfo.LastDistributeHeight,
            Share = delegationInfo.Share,
            StakeInfo = delegationInfo.StakeInfo,
        };
    }

    public static implicit operator DelegatorInfoProto(DelegatorInfo delegationInfo)
    {
        return new DelegatorInfoProto
        {
            LastDistributeHeight = delegationInfo.LastDistributeHeight,
            Share = delegationInfo.Share,
            StakeInfo = delegationInfo.StakeInfo,
        };
    }
}
