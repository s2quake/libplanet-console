using Libplanet.Action.State;
using LibplanetConsole.Common;

namespace LibplanetConsole.Bank.Actions;

[ActionType(TypeIdentifier)]
public sealed class MintAction : ActionBase
{
    private const string TypeIdentifier = "mint_action";

    public MintAction(Address address, decimal amount)
    {
        Address = address;
        Amount = amount;
    }

    public MintAction()
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
        base.OnLoadPlainValue(values);
        Address = new Address(values["address"]);
        Amount = decimal.Parse((Text)values["amount"]);
        Currency = new Currency(values["currency"]);
    }

    protected override IWorld OnExecute(IActionContext context)
    {
        var world = context.PreviousState;
        var value = AssetUtility.GetValue(Currency, Amount);
        return world.MintAsset(context, Address, value);
    }
}
