using Libplanet.Action.State;
using LibplanetConsole.Common;

namespace LibplanetConsole.Bank.Actions;

[ActionType(TypeIdentifier)]
public sealed class BurnAction : ActionBase
{
    private const string TypeIdentifier = "burn_action";

    public BurnAction(Address address, FungibleAssetValue amount)
    {
        Address = address;
        Amount = amount;
    }

    public BurnAction()
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
        Address = new Address(values["address"]);
        Amount = new FungibleAssetValue(values["amount"]);
    }

    protected override IWorld OnExecute(IActionContext context)
    {
        var world = context.PreviousState;
        var value = Amount;
        return world.BurnAsset(context, Address, value);
    }
}
