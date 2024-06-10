using Libplanet.Crypto;
using LibplanetConsole.Common;
using Nekoyume.Action.DPoS.Model;

namespace LibplanetConsole.Nodes.Stakings.Extensions;

public static class AppAddressExtensions
{
    public static AppAddress DeriveAsValidator(this AppAddress @this)
        => (AppAddress)Validator.DeriveAddress((Address)@this);
}
