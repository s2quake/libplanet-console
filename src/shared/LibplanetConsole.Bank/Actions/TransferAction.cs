using Libplanet.Action.State;
using LibplanetConsole.Common;

namespace LibplanetConsole.Bank.Actions;

[ActionType(TypeIdentifier)]
public sealed class TransferAction : ActionBase
{
    private const string TypeIdentifier = "transfer_action";

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
            .Add("address", TargetAddress.Bencoded)
            .Add("amount", Amount.Serialize());
    }

    protected override void OnLoadPlainValue(Dictionary values)
    {
        base.OnLoadPlainValue(values);
        TargetAddress = new Address(values["address"]);
        Amount = new FungibleAssetValue(values["amount"]);
    }

    protected override IWorld OnExecute(IActionContext context)
    {
        var world = context.PreviousState;
        var sender = context.Signer;
        var recipient = TargetAddress;
        var value = Amount;
        return world.TransferAsset(context, sender, recipient, value);
    }
}
