using Libplanet.Action.State;
using LibplanetConsole.Common;

namespace LibplanetConsole.Bank.Actions;

[ActionType(TypeIdentifier)]
public sealed class TransferAction : ActionBase
{
    private const string TypeIdentifier = "transfer_action";

    public TransferAction(Address targetAddress, decimal amount)
    {
        TargetAddress = targetAddress;
        Amount = amount;
    }

    public TransferAction()
    {
    }

    public Address TargetAddress { get; set; }

    public decimal Amount { get; set; }

    public Currency Currency { get; set; } = AssetUtility.NCG;

    protected override Dictionary OnInitialize(Dictionary values)
    {
        return base.OnInitialize(values)
            .Add("address", TargetAddress.Bencoded)
            .Add("amount", $"{Amount:R}")
            .Add("currency", Currency.Serialize());
    }

    protected override void OnLoadPlainValue(Dictionary values)
    {
        base.OnLoadPlainValue(values);
        TargetAddress = new Address(values["address"]);
        Amount = decimal.Parse((Text)values["amount"]);
        Currency = new Currency(values["currency"]);
    }

    protected override IWorld OnExecute(IActionContext context)
    {
        var world = context.PreviousState;
        var sender = context.Signer;
        var recipient = TargetAddress;
        var value = AssetUtility.GetValue(Currency, Amount);
        return world.TransferAsset(context, sender, recipient, value);
    }
}
