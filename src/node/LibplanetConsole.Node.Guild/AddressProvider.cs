using System.Collections.Immutable;
using System.Numerics;
using System.Reflection;
using JSSoft.Commands;
using Libplanet.Types.Consensus;
using LibplanetConsole.Common.Actions;
using LibplanetConsole.Node.Guild.BlockActions;
using Nekoyume;
using Nekoyume.Action;
using Nekoyume.Action.Guild;
using Nekoyume.Action.Loader;
using Nekoyume.Action.ValidatorDelegation;
using Nekoyume.Model.State;
using Nekoyume.TableData;
using static Nekoyume.Action.MintAssets;

namespace LibplanetConsole.Node.Guild;

internal sealed class AddressProvider : IAddressProvider
{
    public IEnumerable<AddressInfo> AddressInfos => GetAddressInfos();

    private static IEnumerable<AddressInfo> GetAddressInfos()
    {
        var fieldInfos = typeof(Addresses).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.FieldType == typeof(Address));

        foreach (var fieldInfo in fieldInfos)
        {
            var address = (Address)fieldInfo.GetValue(null)!;
            var name = CommandUtility.ToSpinalCase(fieldInfo.Name);
            yield return new AddressInfo
            {
                Alias = name,
                Address = address,
                Tags = ["lib9c"],
            };
        }
    }
}
