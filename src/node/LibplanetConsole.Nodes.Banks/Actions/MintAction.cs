using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Crypto;
using Libplanet.Types.Assets;
using LibplanetConsole.Common;
using Nekoyume.Action.DPoS.Util;

namespace LibplanetConsole.Nodes.Banks.Actions;

[ActionType(ActionTypeValue)]
public sealed class MintAction : ActionBase
{
    private const string ActionTypeValue = "mint_action";

    public MintAction(Address address, FungibleAssetValue amount)
    {
        Address = address;
        Amount = amount;
    }

    public MintAction()
    {
    }

    public Address Address { get; set; }

    public FungibleAssetValue Amount { get; set; }

    protected override Dictionary OnInitialize(Dictionary values)
    {
        return base.OnInitialize(values)
            .Add("address", Address.Serialize())
            .Add("amount", Amount.Serialize());
    }

    protected override void OnLoadPlainValue(Dictionary values)
    {
        base.OnLoadPlainValue(values);
        Address = values["address"].ToAddress();
        Amount = values["amount"].ToFungibleAssetValue();
    }

    protected override IWorld OnExecute(IActionContext context)
    {
        IActionContext ctx = context;
        var states = ctx.PreviousState;
        return states.MintAsset(context, Address, Amount);
    }
}
