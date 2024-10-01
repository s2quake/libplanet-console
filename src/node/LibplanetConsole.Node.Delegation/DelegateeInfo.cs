#if LIBPLANET_NODE
using Nekoyume.Delegation;

namespace LibplanetConsole.Delegation;

public readonly partial record struct DelegateeInfo
{
    public DelegateeInfo(DelegateeMetadata metadata)
    {
        Address = metadata.Address;
        TotalDelegated = $"{metadata.TotalDelegatedFAV}";
        TotalShares = $"{metadata.TotalShares}";
        Jailed = metadata.Jailed;
        JailedUntil = metadata.JailedUntil;
        Tombstoned = metadata.Tombstoned;
    }
}
#endif // LIBPLANET_NODE
