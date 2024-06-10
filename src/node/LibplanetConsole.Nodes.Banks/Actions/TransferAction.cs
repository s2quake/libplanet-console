using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Crypto;
using Libplanet.Types.Assets;
using LibplanetConsole.Common;
using Nekoyume.Action.DPoS.Util;

namespace LibplanetConsole.Nodes.Banks.Actions;

[ActionType(ActionTypeValue)]
public sealed class TransferAction : ActionBase
{
    private const string ActionTypeValue = "transfer_action";

    public TransferAction(Address targetAddress, FungibleAssetValue amount)
    {
        TargetAddress = targetAddress;
        Amount = amount;
    }

    public TransferAction()
    {
    }

    public Address TargetAddress { get; set; }

    public FungibleAssetValue Amount { get; set; }

    protected override Dictionary OnInitialize(Dictionary values)
    {
        return base.OnInitialize(values)
            .Add("address", TargetAddress.Serialize())
            .Add("amount", Amount.Serialize());
    }

    protected override void OnLoadPlainValue(Dictionary values)
    {
        base.OnLoadPlainValue(values);
        TargetAddress = values["address"].ToAddress();
        Amount = values["amount"].ToFungibleAssetValue();
    }

    protected override IWorld OnExecute(IActionContext context)
    {
        IActionContext ctx = context;
        var states = ctx.PreviousState;
        return states.TransferAsset(ctx, ctx.Signer, TargetAddress, Amount);
    }
}
