using Libplanet.Action.State;
using Libplanet.Crypto;
using Nekoyume.Action.DPoS.Control;
using Newtonsoft.Json;

namespace LibplanetConsole.Stakings.Serializations;

public record class UndelegationInfo
{
    public UndelegationInfo(IWorldState worldState, Address undelegationAddress)
    {
        if (UndelegateCtrl.GetUndelegation(worldState, undelegationAddress) is not { } undelegation)
        {
            throw new ArgumentException("Undelegation not found.", nameof(undelegationAddress));
        }

        Address = $"{undelegationAddress}";
        DelegatorAddress = $"{undelegation.DelegatorAddress}";
        ValidatorAddress = $"{undelegation.ValidatorAddress}";
        Entires = undelegation.UndelegationEntryAddresses.Values.Select(item =>
        {
            return new UndelegationEntryInfo(worldState, item);
        }).ToArray();
    }

    public UndelegationInfo()
    {
    }

    [JsonIgnore]
    public string Address { get; init; } = string.Empty;

    public string DelegatorAddress { get; init; } = string.Empty;

    public string ValidatorAddress { get; init; } = string.Empty;

    public UndelegationEntryInfo[] Entires { get; init; } = [];
}
