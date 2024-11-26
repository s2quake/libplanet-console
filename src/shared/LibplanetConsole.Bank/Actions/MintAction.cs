using Libplanet.Action.State;
using LibplanetConsole.Common;

namespace LibplanetConsole.Bank.Actions;

[ActionType(TypeIdentifier)]
public sealed class MintAction : ActionBase
{
    private const string TypeIdentifier = "mint_action";

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
            .Add("address", Address.Bencoded)
            .Add("amount", Amount.Serialize());
    }

    protected override void OnLoadPlainValue(Dictionary values)
    {
        base.OnLoadPlainValue(values);
        Address = new Address(values["address"]);
        Amount = new FungibleAssetValue(values["amount"]);
    }

    protected override IWorld OnExecute(IActionContext context)
    {
        var world = context.PreviousState;
        var value = Amount;
        return world.MintAsset(context, Address, value);
    }
}
