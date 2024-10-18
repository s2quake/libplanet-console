using System.Diagnostics.CodeAnalysis;
using Bencodex.Types;
using Libplanet.Action.State;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using Nekoyume;
using Nekoyume.Model.Guild;

namespace LibplanetConsole.Guild;

public readonly record struct GuildInfo
{
    public GuildInfo(IWorldState worldState, Address address)
    {
        var account = worldState.GetAccountState(Addresses.Guild);
        // if (value is not List list)
        // {
        //     throw new ArgumentException("Invalid value type.", nameof(value));
        // }

        // var guild = new Nekoyume.Model.Guild.Guild(list);
        // Address = guild.GuildMasterAddress;
    }

    public Address GuildMasterAddress { get; init; }

    public Address Address { get; init; }
}
