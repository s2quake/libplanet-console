using Libplanet.Action.State;
using LibplanetConsole.Common;

namespace LibplanetConsole.Bank.Actions;

[ActionType(TypeIdentifier)]
public sealed class BurnAction : ActionBase
{
    private const string TypeIdentifier = "burn_action";

    public BurnAction(Address address, decimal amount)
    {
        Address = address;
        Amount = amount;
    }

    public BurnAction()
    {
    }

    public Address Address { get; set; }

    public decimal Amount { get; set; }

    public Currency Currency { get; set; } = AssetUtility.NCG;

    protected override Dictionary OnInitialize(Dictionary values)
    {
        return base.OnInitialize(values)
            .Add("address", Address.Bencoded)
            .Add("amount", $"{Amount:R}")
            .Add("currency", Currency.Serialize());
    }

    protected override void OnLoadPlainValue(Dictionary values)
    {
        Address = new Address(values["address"]);
        Amount = decimal.Parse((Text)values["amount"]);
        Currency = new Currency(values["currency"]);
    }

    protected override IWorld OnExecute(IActionContext context)
    {
        var world = context.PreviousState;
        var value = AssetUtility.GetValue(Currency, Amount);
        return world.BurnAsset(context, Address, value);
    }
}
