using LibplanetConsole.Delegation.Grpc;

namespace LibplanetConsole.Delegation;

public readonly record struct DelegatorInfo(
    long LastDistributeHeight,
    string Share,
    string FAV)
{
    public static implicit operator DelegatorInfo(DelegatorInfoProto delegatorInfo) => new()
    {
        LastDistributeHeight = delegatorInfo.LastDistributeHeight,
        Share = delegatorInfo.Share,
        FAV = delegatorInfo.Fav,
    };

    public static implicit operator DelegatorInfoProto(DelegatorInfo delegatorInfo) => new()
    {
        LastDistributeHeight = delegatorInfo.LastDistributeHeight,
        Share = delegatorInfo.Share,
        Fav = delegatorInfo.FAV,
    };
}
